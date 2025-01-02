using System.Globalization;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isIncome)
        {
            return isIncome ? Application.Current.Resources["Danger"] : Application.Current.Resources["Success"];
        }
        return Application.Current.Resources["Gray900"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}