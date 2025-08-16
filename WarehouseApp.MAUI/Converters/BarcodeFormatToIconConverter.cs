using System.Globalization;
using ZXing.Net.Maui;

namespace WarehouseApp.MAUI.Converters;

public class BarcodeFormatToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not BarcodeFormat format)
            return "📱";

        return format switch
        {
            BarcodeFormat.QrCode => "📱",
            BarcodeFormat.Ean13 => "🏷️",
            BarcodeFormat.Ean8 => "🏷️",
            BarcodeFormat.Code128 => "📊",
            BarcodeFormat.Code39 => "📊",
            BarcodeFormat.Code93 => "📊",
            BarcodeFormat.Codabar => "📊",
            BarcodeFormat.DataMatrix => "⬛",
            BarcodeFormat.Pdf417 => "📄",
            BarcodeFormat.Aztec => "🔷",
            _ => "📱"
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}