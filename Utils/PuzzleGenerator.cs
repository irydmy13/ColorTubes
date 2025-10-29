using System.Collections.ObjectModel;
using ColorTubes.Models;

namespace ColorTubes;

/// <summary>
/// Генератор головоломок Water Sort:
/// N пробирок, цветов = N-2, у каждого цвета 4 юнита, 2 пробирки пустые.
/// </summary>
public static class PuzzleGenerator
{
    // Пастельная палитра (можно расширять)
    private static readonly string[] Palette = new[]
    {
        "#FF9EC4", "#D65A5A", "#FFD166", "#8C6FF7",
        "#7AD7E0", "#67C587", "#5876FF", "#8ED3F5",
        "#FFB3BA", "#A1E3A1", "#B5B5FF", "#FFC58F"
    };

    private static readonly Random Rng = new();

    public static List<Tube> Generate(int tubeCount)
    {
        if (tubeCount < 3) tubeCount = 3;
        // По правилам: 2 пустые
        int colorCount = Math.Max(1, tubeCount - 2);
        if (colorCount > Palette.Length) colorCount = Palette.Length;

        // Собираем все 4*colorCount юнитов
        var units = new List<string>(colorCount * 4);
        for (int i = 0; i < colorCount; i++)
            for (int k = 0; k < 4; k++)
                units.Add(Palette[i]);

        // перемешиваем
        Shuffle(units);

        // заполняем N-2 пробирок, по 4 в каждую
        var tubes = new List<Tube>();
        int idx = 0;
        for (int t = 0; t < tubeCount - 2; t++)
        {
            var segs = new ObservableCollection<LiquidSegment>();
            for (int s = 0; s < 4; s++)
                segs.Add(new LiquidSegment { ColorHex = units[idx++], Amount = 1 });
            tubes.Add(new Tube { Segments = segs });
        }

        // 2 пустые
        tubes.Add(new Tube());
        tubes.Add(new Tube());

        // Небольшая защита: если сгенерили уже решённое (редко) — перегенерим
        if (IsSolved(tubes))
            return Generate(tubeCount);

        return tubes;
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // Проверка решённости (такая же, как в VM)
    private static bool IsSolved(IEnumerable<Tube> tubes)
    {
        foreach (var t in tubes)
        {
            if (t.IsEmpty) continue;
            if (t.FilledAmount != Tube.Capacity) return false;
            var color = t.Segments[0].ColorHex;
            if (t.Segments.Any(s => s.ColorHex != color)) return false;
        }
        return true;
    }
}
