using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ColorTubes.Models;
using ColorTubes.Services;
using ColorTubes.Utils;

namespace ColorTubes.ViewModels;

public class GameViewModel : BaseViewModel
{
    private const int Capacity = 4;

    private readonly DatabaseService _db;

    public ObservableCollection<Tube> Tubes { get; } = new();

    // --- ходов счетчик ---
    private int _moves;
    public int Moves { get => _moves; private set => Set(ref _moves, value); }
    private int _selectedIndex = -1;
    public int SelectedIndex { get => _selectedIndex; set => Set(ref _selectedIndex, value); }


    // имя игрока и индекс уровня (для таблицы рекордов/алертов)
    private string _playerName = "Player";
    private int _levelIndex = 1;

    // история состояний для Undo (JSON снимки)
    private readonly Stack<string> _history = new();
    private string? _initialSnapshot;

    // таймер
    private readonly Stopwatch _watch = new();
    private bool _timerTicking;

    private TimeSpan _elapsed;
    public TimeSpan Elapsed
    {
        get => _elapsed;
        private set => Set(ref _elapsed, value);
    }


    // выбранная колба
    private Tube? _selected;

    // кампания: 5 уровней (5..9 колб)
    public int CurrentLevel { get; private set; } = 1;
    public int TotalLevels { get; } = 5;

    // масштаб отрисовки колб (уменьшается при росте их числа)
    private double _tubeScale = 1.0;
    public double TubeScale
    {
        get => _tubeScale;
        private set => Set(ref _tubeScale, value);
    }

    // Команды
    public ICommand SelectTubeCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand UndoCommand { get; }

    public GameViewModel(DatabaseService db)
    {
        _db = db;

        SelectTubeCommand = new Command<Tube>(OnSelectTube);
        ResetCommand = new Command(ResetLevel);
        UndoCommand = new Command(Undo, () => _history.Count > 1);
    }

    // ===== Старт игры (одиночный уровень) =====
    public async Task StartNewGameAsync(int tubeCount, int levelIndex, string playerName)
    {
        _playerName = string.IsNullOrWhiteSpace(playerName) ? "Player" : playerName.Trim();
        _levelIndex = levelIndex;

        // генерируем раскладку
        LoadTubes(LevelLayouts.BuildRandomTubes(tubeCount));

        // снимок для Reset
        _initialSnapshot = LevelLayouts.ToJson(Tubes.ToList());
        _history.Clear();
        _history.Push(_initialSnapshot);
        UpdateUndoState();

        Moves = 0;
        _selected = null;

        StartTimer();

        await Task.CompletedTask;
    }

    // ===== Кампания 5 уровней (5..9 колб) =====
    public async Task StartCampaignAsync()
    {
        CurrentLevel = 1;
        await StartLevelAsync(CurrentLevel);
    }

    public async Task StartLevelAsync(int level)
    {
        _levelIndex = level;
        int tubesCount = 5 + (level - 1); // 5..9

        LoadTubes(LevelLayouts.BuildRandomTubes(tubesCount));

        _initialSnapshot = LevelLayouts.ToJson(Tubes.ToList());
        _history.Clear();
        _history.Push(_initialSnapshot);
        UpdateUndoState();

        Moves = 0;
        _selected = null;

        // масштаб
        TubeScale = tubesCount <= 6 ? 1.0 : tubesCount <= 8 ? 0.9 : 0.8;

        StartTimer();
        await Task.CompletedTask;
    }

    // ===== Таймер =====
    private void StartTimer()
    {
        _watch.Reset();
        _watch.Start();
        if (_timerTicking) return;

        _timerTicking = true;
        Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
        {
            Elapsed = _watch.Elapsed;
            return _timerTicking;
        });
    }

    private void StopTimer()
    {
        _watch.Stop();
        _timerTicking = false;
        Elapsed = _watch.Elapsed;
    }

    // ===== Undo / Reset / Snapshot =====
    private void SaveSnapshot()
    {
        _history.Push(LevelLayouts.ToJson(Tubes.ToList()));
        UpdateUndoState();
    }

    private void UpdateUndoState()
    {
        (UndoCommand as Command)?.ChangeCanExecute();
    }

    private void ResetLevel()
    {
        if (string.IsNullOrEmpty(_initialSnapshot)) return;
        LoadFromJson(_initialSnapshot);
        _history.Clear();
        _history.Push(_initialSnapshot);
        UpdateUndoState();

        Moves = 0;
        _selected = null;

        _watch.Restart();
    }

    private void Undo()
    {
        if (_history.Count <= 1) return;
        _history.Pop();
        LoadFromJson(_history.Peek());
        _selected = null;
        UpdateUndoState();
    }

    private void LoadFromJson(string json)
    {
        LoadTubes(LevelLayouts.FromJson(json));
    }

    private void LoadTubes(List<Tube> tubes)
    {
        Tubes.Clear();
        foreach (var t in tubes) Tubes.Add(t);
    }

    // ===== Выбор/перелив =====
    private void OnSelectTube(Tube tube)
    {
        if (_selected is null)
        {
            if (!IsEmpty(tube)) _selected = tube;
            return;
        }

        if (ReferenceEquals(_selected, tube))
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
            _selected = IsEmpty(tube) ? null : tube;
        }
    }

    private bool TryPour(Tube from, Tube to)
    {
        if (IsEmpty(from) || IsFull(to) || ReferenceEquals(from, to)) return false;

        var color = TopColor(from);
        if (color is null) return false;

        var toTop = TopColor(to);
        if (toTop is not null && toTop != color) return false;

        int movable = CountMovableTop(from, color);
        int free = FreeAmount(to);
        int move = Math.Min(movable, free);
        if (move <= 0) return false;

        SaveSnapshot();

        int remain = move;
        while (remain > 0)
        {
            var top = from.Segments[0];
            int step = Math.Min(top.Amount, remain);
            top.Amount -= step;
            if (top.Amount == 0)
                from.Segments.RemoveAt(0);

            if (to.Segments.Count > 0 && to.Segments[0].ColorHex == color)
                to.Segments[0].Amount += step;          // сливаем с верхним слоем
            else
                to.Segments.Insert(0, new LiquidSegment { ColorHex = color, Amount = step });

            remain -= step;
        }

        MergeTop(from);
        MergeTop(to);
        return true;
    }

    private static void MergeTop(Tube t)
    {
        if (t.Segments.Count < 2) return;
        var top = t.Segments[0];
        var next = t.Segments[1];
        if (top.ColorHex == next.ColorHex)
        {
            next.Amount += top.Amount;
            t.Segments.RemoveAt(0);
        }
    }

    // ===== Проверка победы =====
    private bool IsSolved()
    {
        foreach (var t in Tubes)
        {
            if (IsEmpty(t)) continue;
            if (FilledAmount(t) != Capacity) return false;
            var first = t.Segments[0].ColorHex;
            if (t.Segments.Any(s => s.ColorHex != first)) return false;
        }
        return true;
    }

    private async Task OnWinAsync()
    {
        StopTimer();

        // сохраняем результат за текущий уровень
        await _db.SaveScoreAsync(new PlayerScore
        {
            PlayerName = _playerName,
            Moves = Moves,
            PlayTime = _watch.Elapsed.TotalSeconds,
            LevelIndex = _levelIndex   // если поле есть в модели
        });

        string timeStr = Elapsed.ToString(Elapsed.Hours > 0 ? @"h\:mm\:ss" : @"m\:ss");
        await Application.Current.MainPage.DisplayAlert(
            "Уровень пройден!",
            $"Уровень {_levelIndex} завершён\nИмя: {_playerName}\nХоды: {Moves}\nВремя: {timeStr}",
            "OK");

        // если это был последний уровень кампании — показываем финальный диалог
        if (CurrentLevel >= TotalLevels)
        {
            bool goRating = await Application.Current.MainPage.DisplayAlert(
                "Победа! 🎉",
                $"Ты прошёл все {TotalLevels} уровней. Молодец!",
                "Перейти в рейтинги",
                "Закрыть");

            if (goRating)
            {
                // переход на страницу рейтинга (как и просила)
                await Shell.Current.GoToAsync("///main/rating");
            }
            return; // выходим, не запускаем следующий уровень
        }

        // иначе — продолжаем кампанию
        CurrentLevel++;
        await StartLevelAsync(CurrentLevel);
    }

    // ===== хелперы по колбе =====
    private static int FilledAmount(Tube t) => t.Segments.Sum(s => s.Amount);
    private static int FreeAmount(Tube t) => Math.Max(0, Capacity - FilledAmount(t));
    private static bool IsEmpty(Tube t) => t.Segments.Count == 0;
    private static bool IsFull(Tube t) => FilledAmount(t) >= Capacity;
    private static string? TopColor(Tube t) => IsEmpty(t) ? null : t.Segments[0].ColorHex;

    private static int CountMovableTop(Tube t, string color)
    {
        int cnt = 0;
        for (int i = 0; i < t.Segments.Count; i++)
        {
            var seg = t.Segments[i];
            if (seg.ColorHex != color) break;
            cnt += seg.Amount;
        }
        return cnt;
    }

    // ===== Демо-уровень (если нужно) =====
    public async Task StartDefaultLevelAsync()
    {
        var level = LevelLayouts.SampleLevel();
        LoadTubes(LevelLayouts.FromJson(level.LayoutJson));

        _initialSnapshot = LevelLayouts.ToJson(Tubes.ToList());
        _history.Clear();
        _history.Push(_initialSnapshot);
        UpdateUndoState();

        Moves = 0;
        _selected = null;

        StartTimer();
        await Task.CompletedTask;
    }

    public async Task EnsureStartedAsync()
    {
        if (Tubes.Count == 0)
            await StartDefaultLevelAsync();
    }
}
