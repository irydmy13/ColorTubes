using System.Collections.ObjectModel;
using System.Text.Json;
using ColorTubes.Models;

namespace ColorTubes;

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

    // ПАЛИТРА «как на картинке»
    private const string PINK = "#FF9EC4"; // розовый
    private const string RED = "#D65A5A"; // красный
    private const string YEL = "#FFD166"; // жёлтый
    private const string PURP = "#8C6FF7"; // фиолетовый
    private const string TEAL = "#7AD7E0"; // бирюзовый
    private const string GREEN = "#67C587"; // зелёный
    private const string BLUE = "#5876FF"; // синий
    private const string CYAN = "#8ED3F5"; // голубой

    private static Tube T(params string?[] topToBottom)
    {
        var segs = new ObservableCollection<LiquidSegment>();
        foreach (var c in topToBottom)
            if (c != null) segs.Add(new() { ColorHex = c, Amount = 1 });
        return new Tube { Segments = segs };
    }

    public static Level SampleLevel()
    {
        // Верхний ряд (6)
        var t1 = T(PINK, RED, YEL, PURP);
        var t2 = T(PINK, RED, YEL, GREEN);
        var t3 = T(PURP, TEAL, YEL, null);
        var t4 = T(BLUE, BLUE, GREEN, CYAN);   // двойной синий сверху
        var t5 = T(BLUE, GREEN, GREEN, CYAN);  // высокий зелёный
        var t6 = T(BLUE, BLUE, RED, CYAN);   // два синих + красный + голубой

        // Нижний ряд (2 + 3 пустые)
        var t7 = T(YEL, RED, PINK, PURP);
        var t8 = T(TEAL, PINK, GREEN, PURP);

        var tubes = new List<Tube> { t1, t2, t3, t4, t5, t6, t7, t8, new Tube(), new Tube(), new Tube() };

        return new Level { Name = "Дизайн-демо (как на картинке)", LayoutJson = ToJson(tubes) };
    }
}
