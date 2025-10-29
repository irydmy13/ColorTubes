using System.Globalization;

namespace ColorTubes.Converters;

public sealed class EnumEqualsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null) return false;
        var enumType = value.GetType();
        var param = Enum.Parse(enumType, parameter.ToString()!);
        return value.Equals(param);
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is null) return Binding.DoNothing;
        return Enum.Parse(targetType, parameter.ToString()!);
    }
}
