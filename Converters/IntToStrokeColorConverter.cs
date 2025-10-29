using System.Globalization;
using Microsoft.Maui.Graphics;

namespace ColorTubes.Converters;

// Если Segments.Count > 0 — возвращает белую обводку (или переданный цвет),
// иначе полупрозрачную
public class IntToStrokeColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        int count = value is int i ? i : 0;
        var full = parameter is Color c ? c : Colors.White;
        return count > 0 ? full : Color.FromArgb("#80FFFFFF");
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
