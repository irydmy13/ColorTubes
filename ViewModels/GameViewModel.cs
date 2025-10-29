using System.Collections.ObjectModel;
using System.Windows.Input;
using ColorTubes.Models;
using ColorTubes.Services;

namespace ColorTubes.ViewModels;

//VM ����: ������ ��������, ������� ����, ��������� ������� �����������.

public class GameViewModel : BaseViewModel
{
    private readonly DatabaseService _db;

    public ObservableCollection<Tube> Tubes { get; } = new();
    public int Moves { get => _moves; private set => Set(ref _moves, value); }
    private int _moves;

    private Tube? _selected;

    // ������� ��� Undo: ���� ��������� (JSON)
    private readonly Stack<string> _history = new();

    public ICommand SelectTubeCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand UndoCommand { get; }

    public GameViewModel(DatabaseService db)
    {
        _db = db;
        SelectTubeCommand = new Command<Tube>(OnSelectTube);
        ResetCommand = new Command(ResetLevel);
        UndoCommand = new Command(Undo, () => _history.Count > 0);

        _ = LoadSampleAsync();
    }

    private async Task LoadSampleAsync()
    {
        await _db.EnsureSampleLevelAsync();
        var level = (await _db.GetLevelsAsync()).First();
        LoadLevelFromJson(level.LayoutJson);
    }

    public void LoadLevelFromJson(string json)
    {
        Tubes.Clear();
        foreach (var t in LevelLayouts.FromJson(json))
            Tubes.Add(t);
        _selected = null;
        _history.Clear();
        Moves = 0;
    }

    private void SaveSnapshot() => _history.Push(LevelLayouts.ToJson(Tubes.Select(t => t.Clone()).ToList()));

    private void Undo()
    {
        if (_history.Count == 0) return;
        var json = _history.Pop();
        LoadLevelFromJson(json);
    }

    private void ResetLevel()
    {
        if (_history.Count == 0) return;
        var first = _history.Last();   // ����� ������ ������
        _history.Clear();
        LoadLevelFromJson(first);
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
        }
        else
        {
            // ���� ������ � �������� �����
            _selected = tube.IsEmpty ? null : tube;
        }
    }

    //������� �����������:
    // - ������ ���� � ������ ��������
    // - ������ ���� �� ������
    // - ���� ������ ��������� � ������� ������ ���������� (��� ���������� ������)
    // - ���������� ������ ������� ���� (��� ���� ����� ����� ������), ���� ���� �����
    private bool TryPour(Tube from, Tube to)
    {
        if (from.IsEmpty || to.IsFull || from == to) return false;

        var color = from.TopColor!;
        if (!to.IsEmpty && to.TopColor != color) return false;

        // ������� ������ ���������� ���� ������ � from?
        int movable = 0;
        int i = from.Segments.Count - 1;
        while (i >= 0 && from.Segments[i].ColorHex == color)
        {
            movable += from.Segments[i].Amount;
            i--;
        }

        if (movable == 0) return false;

        int canMove = Math.Min(movable, to.FreeAmount);
        if (canMove == 0) return false;

        // ---- ������ ��� Undo
        SaveSnapshot();

        // ��������� ���� �� ����� (����������� ������)
        int remaining = canMove;
        while (remaining > 0)
        {
            var top = from.Segments[^1];
            int step = Math.Min(top.Amount, remaining);

            // ��������� ������� � from
            top.Amount -= step;
            if (top.Amount == 0)
                from.Segments.RemoveAt(from.Segments.Count - 1);

            // ���������/���������� � to
            if (!to.IsEmpty && to.TopColor == color)
                to.Segments[^1].Amount += step;
            else
                to.Segments.Add(new LiquidSegment { ColorHex = color, Amount = step });

            remaining -= step;
        }

        // ������� ���������� �������� (�� ������ ������)
        MergeTop(to);
        MergeTop(from);

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
}
