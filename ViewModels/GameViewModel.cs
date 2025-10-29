using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using ColorTubes.Models;
using ColorTubes.Services;

namespace ColorTubes.ViewModels;

public class GameViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<Tube> Tubes { get; } = new();

    private int _moves;
    public int Moves { get => _moves; private set => Set(ref _moves, value); }

    private string _playerName = "Player";
    private int _levelIndex = 1;

    private readonly Stack<string> _history = new();
    private readonly Stopwatch _timer = new();

    public ICommand SelectTubeCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand UndoCommand { get; }

    private Tube? _selected;

    public GameViewModel(DatabaseService db)
    {
        _db = db;
        SelectTubeCommand = new Command<Tube>(OnSelectTube);
        ResetCommand = new Command(ResetLevel);
        UndoCommand = new Command(Undo, () => _history.Count > 1);
    }

    public async Task StartNewGameAsync(int tubeCount, int levelIndex, string playerName)
    {
        _playerName = playerName;
        _levelIndex = levelIndex;

        // генерируем новую головоломку
        Tubes.Clear();
        foreach (var t in PuzzleGenerator.Generate(tubeCount))
            Tubes.Add(t);

        _history.Clear();
        SaveSnapshot();
        Moves = 0;
        _selected = null;

        _timer.Reset();
        _timer.Start();

        await Task.CompletedTask;
    }

    private void SaveSnapshot()
    {
        var list = Tubes.Select(t => t.Clone()).ToList();
        _history.Push(LevelLayouts.ToJson(list));
        (UndoCommand as Command)?.ChangeCanExecute();
    }

    private void ResetLevel()
    {
        if (_history.Count == 0) return;
        var first = _history.First();
        LoadFromJson(first);
        Moves = 0;
        _timer.Restart();
    }

    private void Undo()
    {
        if (_history.Count <= 1) return;
        _history.Pop();
        var json = _history.Peek();
        LoadFromJson(json);
        (UndoCommand as Command)?.ChangeCanExecute();
    }

    private void LoadFromJson(string json)
    {
        Tubes.Clear();
        foreach (var t in LevelLayouts.FromJson(json))
            Tubes.Add(t);
        _selected = null;
    }

    private void OnSelectTube(Tube tube)
    {
        if (_selected is null)
        {
            if (!tube.IsEmpty) _selected = tube;
            return;
        }

        if (_selected == tube)
        {
            _selected = null;
            return;
        }

        if (TryPour(_selected, tube))
        {
            Moves++;
            _selected = null;
            if (IsSolved())
                _ = OnWinAsync();
        }
        else
        {
            _selected = tube.IsEmpty ? null : tube;
        }
    }

    private bool TryPour(Tube from, Tube to)
    {
        if (from.IsEmpty || to.IsFull || from == to) return false;

        var color = from.TopColor!;
        if (!to.IsEmpty && to.TopColor != color) return false;

        // сколько одинакового цвета сверху у from
        int movable = 0;
        for (int i = from.Segments.Count - 1; i >= 0; i--)
        {
            if (from.Segments[i].ColorHex == color) movable += from.Segments[i].Amount;
            else break;
        }

        int canMove = Math.Min(movable, to.FreeAmount);
        if (canMove == 0) return false;

        SaveSnapshot();

        int remain = canMove;
        while (remain > 0)
        {
            var top = from.Segments[^1];
            int step = Math.Min(top.Amount, remain);
            top.Amount -= step;
            if (top.Amount == 0) from.Segments.RemoveAt(from.Segments.Count - 1);

            if (!to.IsEmpty && to.TopColor == color)
                to.Segments[^1].Amount += step;
            else
                to.Segments.Add(new LiquidSegment { ColorHex = color, Amount = step });

            remain -= step;
        }

        MergeTop(from);
        MergeTop(to);
        return true;
    }

    private static void MergeTop(Tube t)
    {
        if (t.Segments.Count < 2) return;
        var a = t.Segments[^1];
        var b = t.Segments[^2];
        if (a.ColorHex == b.ColorHex)
        {
            b.Amount += a.Amount;
            t.Segments.RemoveAt(t.Segments.Count - 1);
        }
    }

    private bool IsSolved()
    {
        foreach (var t in Tubes)
        {
            if (t.IsEmpty) continue;
            if (t.FilledAmount != Tube.Capacity) return false;
            var color = t.Segments[0].ColorHex;
            if (t.Segments.Any(s => s.ColorHex != color)) return false;
        }
        return true;
    }

    private async Task OnWinAsync()
    {
        _timer.Stop();

        var score = new PlayerScore
        {
            PlayerName = _playerName,
            LevelIndex = _levelIndex,
            Moves = Moves,
            TimeMs = _timer.ElapsedMilliseconds
        };
        await _db.SaveScoreAsync(score);

        string timeStr = TimeSpan.FromMilliseconds(score.TimeMs).ToString(@"m\:ss\.fff");
        await Shell.Current.DisplayAlert("Победа!",
            $"Уровень {_levelIndex} пройден.\n" +
            $"Имя: {_playerName}\nХоды: {Moves}\nВремя: {timeStr}",
            "OK");
    }
}
