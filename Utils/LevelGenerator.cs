using System.Collections.ObjectModel;
using ColorTubes.Models;

namespace ColorTubes;

public static class LevelGenerator
{
    // Пастель – стабильная палитра
    private static readonly string[] Palette = {
        "#FF9EC4","#D65A5A","#FFD166","#8C6FF7",
        "#7AD7E0","#67C587","#5876FF","#8ED3F5",
        "#FFB6A0","#8EE1B1","#B59DFF","#F4B3FF"
    };

    /// <summary>
    /// Генератор уровня: tubes = baseTubes (ур.1=3, ур.2=4 ... ур.5=7).
    /// Два последних – пустые, остальные содержат цвета (каждый цвет по 4 «юнита»).
    /// Не гарантирует 100% решаемость, но для демонстрации подходит.
    /// </summary>
    public static List<Tube> CreateLevel(int levelNumber)
    {
        int tubes = 2 + levelNumber; // ур.1=3 ... ур.5=7
        int colors = Math.Max(1, tubes - 2);

        // Набираем «юнитов» (каждый цвет 4 раза)
        var units = new List<string>();
        for (int i = 0; i < colors; i++)
            for (int k = 0; k < Tube.Capacity; k++)
                units.Add(Palette[i % Palette.Length]);

        // Перемешаем
        var rnd = new Random(levelNumber * 7919);
        for (int i = units.Count - 1; i > 0; i--)
        {
            int j = rnd.Next(i + 1);
            (units[i], units[j]) = (units[j], units[i]);
        }

        // Раскладываем по колбам (кроме двух последних – пустые)
        var tubesList = new List<Tube>();
        int idx = 0;
        for (int t = 0; t < tubes - 2; t++)
        {
            var segs = new ObservableCollection<LiquidSegment>();
            int filled = 0;
            while (filled < Tube.Capacity && idx < units.Count)
            {
                string c = units[idx++];
                // группируем подряд идущие одинаковые цвета в один сегмент
                if (segs.Count > 0 && segs[^1].ColorHex == c && segs[^1].Amount < Tube.Capacity)
                    segs[^1].Amount++;
                else
                    segs.Add(new LiquidSegment { ColorHex = c, Amount = 1 });

                filled = segs.Sum(s => s.Amount);
            }
            tubesList.Add(new Tube { Segments = segs });
        }

        tubesList.Add(new Tube()); // пустая
        tubesList.Add(new Tube()); // пустая

        return tubesList;
    }
}
