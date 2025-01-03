using System.Globalization;

namespace AppMMR.Converters;

public class BoolToIncomeTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isIncome)
        {
            return isIncome ? "收入" : "支出";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 