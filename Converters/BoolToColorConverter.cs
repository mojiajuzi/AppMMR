using System.Globalization;
using Microsoft.Maui.Graphics;

namespace AppMMR.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isIncome)
        {
            if (isIncome)
            {
                // 收入显示红色
                if (Application.Current.Resources.TryGetValue("Danger", out var dangerColor))
                {
                    return (Color)dangerColor;
                }
                return Colors.Red;
            }
            else
            {
                // 支出显示绿色
                if (Application.Current.Resources.TryGetValue("Success", out var successColor))
                {
                    return (Color)successColor;
                }
                return Colors.Green;
            }
        }

        // 默认颜色
        if (Application.Current.Resources.TryGetValue("Gray900", out var defaultColor))
        {
            return (Color)defaultColor;
        }
        return Colors.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}