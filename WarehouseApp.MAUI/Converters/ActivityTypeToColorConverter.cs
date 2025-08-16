using System.Globalization;
using WarehouseApp.MAUI.ViewModels;

namespace WarehouseApp.MAUI.Converters;

public class ActivityTypeToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ActivityType activityType)
        {
            return activityType switch
            {
                ActivityType.StockIn => Colors.Green,
                ActivityType.StockOut => Colors.Orange,
                ActivityType.Transfer => Colors.Blue,
                ActivityType.Create => Colors.Purple,
                ActivityType.Update => Colors.Gray,
                ActivityType.Delete => Colors.Red,
                _ => Colors.Gray
            };
        }
        return Colors.Gray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}