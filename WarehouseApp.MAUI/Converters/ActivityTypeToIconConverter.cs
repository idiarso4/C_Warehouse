using System.Globalization;
using WarehouseApp.MAUI.ViewModels;

namespace WarehouseApp.MAUI.Converters;

public class ActivityTypeToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ActivityType activityType)
        {
            return activityType switch
            {
                ActivityType.StockIn => "↓",
                ActivityType.StockOut => "↑",
                ActivityType.Transfer => "↔",
                ActivityType.Create => "+",
                ActivityType.Update => "✎",
                ActivityType.Delete => "×",
                _ => "•"
            };
        }
        return "•";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}