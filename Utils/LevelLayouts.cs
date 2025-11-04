using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using ColorTubes.Models;

namespace ColorTubes.Utils;

public static class LevelLayouts
{
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static string ToJson(List<Tube> tubes) => JsonSerializer.Serialize(tubes, _json);

    public static List<Tube> FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new();
        var list = JsonSerializer.Deserialize<List<Tube>>(json, _json) ?? new();
        foreach (var t in list)
            t.Segments = new ObservableCollection<LiquidSegment>(t.Segments ?? new());
        return list;
    }

    private const string PINK = "#FF9EC4";   // розовый
    private const string RED = "#D65A5A";   // красный
    private const string YEL = "#FFD166";   // жёлтый
    private const string PURP = "#8C6FF7";   // фиолетовый
    private const string TEAL = "#7AD7E0";   // бирюзовый
    private const string GREEN = "#67C587";   // зелёный
    private const string BLUE = "#5876FF";   // синий
    private const string CYAN = "#8ED3F5";   // голубой

    private static readonly string[] Palette = new[]
    {
        PINK, RED, YEL, PURP, TEAL, GREEN, BLUE, CYAN
    };

    private static Tube T(params string?[] topToBottom)
    {
        // нижний элемент — первый
        var segs = new ObservableCollection<LiquidSegment>();
        // разворачиваем массив, чтобы первый (дно) был в начале
        foreach (var c in topToBottom.Reverse())
            if (c != null) segs.Add(new() { ColorHex = c, Amount = 1 });
        return new Tube { Segments = segs };
    }

    public static Level SampleLevel()
    {
        var t1 = T(PINK, RED, YEL, PURP);
        var t2 = T(PINK, RED, YEL, GREEN);
        var t3 = T(PURP, TEAL, YEL, null);
        var t4 = T(BLUE, BLUE, GREEN, CYAN);
        var t5 = T(BLUE, GREEN, GREEN, CYAN);
        var t6 = T(BLUE, BLUE, RED, CYAN);
        var t7 = T(YEL, RED, PINK, PURP);
        var t8 = T(TEAL, PINK, GREEN, PURP);

        var tubes = new List<Tube> { t1, t2, t3, t4, t5, t6, t7, t8, new Tube(), new Tube(), new Tube() };

        return new Level
        {
            Name = "Дизайн-демо (как на картинке)",
            LayoutJson = ToJson(tubes)
        };
    }

    public static Level BuildRandomLevel(int tubeCount, int seed = 0)
    {
        tubeCount = Math.Max(3, tubeCount);
        const int capacity = 4;

        int colorTubes = Math.Max(1, tubeCount - 2);              // оставляем 2 пустые
        var colors = Palette.Take(colorTubes).ToArray();           // разные цвета на каждую «цветную» колбу

        var pool = new List<string>(colorTubes * capacity);
        foreach (var c in colors)
            for (int i = 0; i < capacity; i++) pool.Add(c);

        var rnd = seed == 0 ? new Random() : new Random(seed);
        pool = pool.OrderBy(_ => rnd.Next()).ToList();

        var tubes = new List<Tube>();
        int idx = 0;
        for (int t = 0; t < colorTubes; t++)
        {
            var segs = new ObservableCollection<LiquidSegment>();
            // берём 4 цвета для этой колбы (порядок в pool — от низа к верху):
            for (int k = 0; k < capacity; k++)
                segs.Add(new LiquidSegment { ColorHex = pool[idx++], Amount = 1 });

            tubes.Add(new Tube { Segments = segs });
        }

        tubes.Add(new Tube { Segments = new ObservableCollection<LiquidSegment>() });
        tubes.Add(new Tube { Segments = new ObservableCollection<LiquidSegment>() });

        return new Level
        {
            Name = $"Level {tubeCount}",
            LayoutJson = ToJson(tubes)
        };
    }

    public static List<Tube> BuildRandomTubes(int tubeCount, int seed = 0)
        => FromJson(BuildRandomLevel(tubeCount, seed).LayoutJson);

    public static List<List<string>> ToColorLists(List<Tube> tubes)
        => JsonSerializer.Deserialize<List<List<string>>>(ToJson(tubes)) ?? new();

    public static List<Tube> FromColorLists(List<List<string>> lists)
        => FromJson(JsonSerializer.Serialize(lists));
}
