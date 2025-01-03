using System.Globalization;
using AppMMR.Models.Enums;

namespace AppMMR.Converters;

public class WorkStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is WorkStatusEnum status)
        {
            return status switch
            {
                WorkStatusEnum.PreStart => Application.Current.Resources["Warning"],    // 黄色
                WorkStatusEnum.InProgress => Application.Current.Resources["Info"],     // 蓝色
                WorkStatusEnum.Completed => Application.Current.Resources["Success"],   // 绿色
                WorkStatusEnum.Cancelled => Application.Current.Resources["Danger"],    // 红色
                _ => Application.Current.Resources["Gray400"]
            };
        }
        return Application.Current.Resources["Gray400"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 