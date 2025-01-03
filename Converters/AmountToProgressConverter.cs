using System.Globalization;

namespace AppMMR.Converters;

public class AmountToProgressConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal amount && parameter is decimal total && total > 0)
        {
            // 返回百分比（0-1之间的值）
            return (double)(amount / total);
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 