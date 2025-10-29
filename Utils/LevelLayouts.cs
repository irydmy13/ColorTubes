using System.Text.Json;
using ColorTubes.Models;

namespace ColorTubes;

/// <summary>
/// Сериализация/десериализация LayoutJson и пример уровня.
/// Формат JSON: массив пробирок; пробирка = массив сегментов { colorHex, amount }.
/// </summary>
public static class LevelLayouts
{
    private static readonly JsonSerializerOptions _json = new()
    {
        // Пишем в camelCase, но ЧИТАЕМ без учёта регистра,
        // чтобы Tube.Segments / LiquidSegment.ColorHex корректно маппились.
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static string ToJson(List<Tube> tubes) => JsonSerializer.Serialize(tubes, _json);

    public static List<Tube> FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new List<Tube>();

        var list = JsonSerializer.Deserialize<List<Tube>>(json, _json) ?? new List<Tube>();

        // Подстраховка: чтобы Segments не были null
        foreach (var t in list)
            t.Segments ??= new List<LiquidSegment>();

        return list;
    }

    public static Level SampleLevel()
    {
        var tubes = new List<Tube>
        {
            new() { Segments = new() {
                new LiquidSegment{ ColorHex="#FF5252", Amount=1},
                new LiquidSegment{ ColorHex="#43A047", Amount=2},
                new LiquidSegment{ ColorHex="#FF5252", Amount=1},
            }},
            new() { Segments = new() {
                new LiquidSegment{ ColorHex="#3D5AFE", Amount=2},
            }},
            new() { Segments = new() {
                new LiquidSegment{ ColorHex="#43A047", Amount=3},
                new LiquidSegment{ ColorHex="#3D5AFE", Amount=1},
            }},
            new(), // пустая
            new(), // пустая
            new() { Segments = new() {
                new LiquidSegment{ ColorHex="#FF5252", Amount=2},
                new LiquidSegment{ ColorHex="#3D5AFE", Amount=1},
            }},
        };

        return new Level
        {
            Name = "Пример",
            LayoutJson = ToJson(tubes)
        };
    }
}
