using System.Globalization;

namespace ColorTubes.Converters;

//amount (1..4) -> высота в пикселях (unit по параметру, дефолт 40)
public class AmountToHeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int amt) return 0;
        int unit = 40;
        if (parameter is string s && int.TryParse(s, out var p)) unit = p;
        return Math.Max(0, amt * unit);
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
