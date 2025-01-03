using System.Globalization;

namespace AppMMR.Converters;

public class ColorWithOpacityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Length == 2 && 
            values[0] is Color baseColor && 
            values[1] is string opacityString &&
            double.TryParse(opacityString, out double opacity))
        {
            return baseColor.WithAlpha((float)opacity);
        }
        return Colors.Transparent;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 