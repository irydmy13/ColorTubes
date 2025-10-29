using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ColorTubes.Models;

// Пробирка. Сегменты храним СНИЗУ ВВЕРХ (последний – верхний слой)
public class Tube : INotifyPropertyChanged
{
    public const int Capacity = 4;

    private ObservableCollection<LiquidSegment> _segments = new();
    public ObservableCollection<LiquidSegment> Segments
    {
        get => _segments;
        set
        {
            if (!ReferenceEquals(_segments, value))
            {
                if (_segments != null)
                {
                    _segments.CollectionChanged -= OnSegmentsChanged;
                    foreach (var s in _segments) s.PropertyChanged -= OnSegmentChanged;
                }
                _segments = value ?? new();
                _segments.CollectionChanged += OnSegmentsChanged;
                foreach (var s in _segments) s.PropertyChanged += OnSegmentChanged;
                OnPropertyChanged();
                RaiseComputedChanged();
            }
        }
    }

    public int FilledAmount => Segments.Sum(s => s.Amount);
    public int FreeAmount => Capacity - FilledAmount;
    public bool IsEmpty => Segments.Count == 0;
    public bool IsFull => FilledAmount >= Capacity;
    public string? TopColor => IsEmpty ? null : Segments[^1].ColorHex;

    public Tube()
    {
        _segments.CollectionChanged += OnSegmentsChanged;
    }

    void OnSegmentsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // подписываемся/отписываемся от изменений Amount внутри сегментов
        if (e.OldItems != null)
            foreach (LiquidSegment s in e.OldItems) s.PropertyChanged -= OnSegmentChanged;
        if (e.NewItems != null)
            foreach (LiquidSegment s in e.NewItems) s.PropertyChanged += OnSegmentChanged;

        RaiseComputedChanged();
    }

    void OnSegmentChanged(object? sender, PropertyChangedEventArgs e)
        => RaiseComputedChanged();

    void RaiseComputedChanged()
    {
        OnPropertyChanged(nameof(FilledAmount));
        OnPropertyChanged(nameof(FreeAmount));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(IsFull));
        OnPropertyChanged(nameof(TopColor));
    }

    public Tube Clone()
        => new()
        {
            Segments = new ObservableCollection<LiquidSegment>(
                Segments.Select(s => new LiquidSegment { ColorHex = s.ColorHex, Amount = s.Amount }))
        };

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
