using System.Globalization;

namespace ColorTubes.Converters;

// Конвертирует Amount (1..4) в высоту слоя.
// ConverterParameter — высота 1 юнита в пикселях (double).

public sealed class AmountToHeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        var amount = value is int i ? i : 0;

        double unit = 48; // по умолчанию
        if (parameter != null &&
            double.TryParse(parameter.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var p))
        {
            unit = p;
        }

        return amount * unit;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}
