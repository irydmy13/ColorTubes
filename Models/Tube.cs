namespace ColorTubes.Models;

//Пробирка. Сегменты храним СНИЗУ ВВЕРХ (последний – верхний слой)
public class Tube
{
    public const int Capacity = 4;

    public List<LiquidSegment> Segments { get; set; } = new();

    public int FilledAmount => Segments.Sum(s => s.Amount);
    public int FreeAmount => Capacity - FilledAmount;
    public bool IsEmpty => Segments.Count == 0;
    public bool IsFull => FilledAmount >= Capacity;

    public string? TopColor => IsEmpty ? null : Segments[^1].ColorHex;

    public Tube Clone()
        => new() { Segments = Segments.Select(s => new LiquidSegment { ColorHex = s.ColorHex, Amount = s.Amount }).ToList() };
}
