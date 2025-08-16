using System.Globalization;

namespace WarehouseApp.MAUI.Converters;

public class BoolToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
            return string.Empty;

        if (parameter is not string parameterString)
            return boolValue.ToString();

        // Parameter format: "TrueValue|FalseValue"
        var parts = parameterString.Split('|');
        if (parts.Length != 2)
            return boolValue.ToString();

        return boolValue ? parts[0] : parts[1];
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}