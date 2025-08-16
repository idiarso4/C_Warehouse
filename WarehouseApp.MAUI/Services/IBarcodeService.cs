using ZXing.Net.Maui;

namespace WarehouseApp.MAUI.Services;

public interface IBarcodeService
{
    /// <summary>
    /// Start barcode scanning
    /// </summary>
    /// <returns>Scanned barcode result or null if cancelled</returns>
    Task<BarcodeResult?> ScanBarcodeAsync();

    /// <summary>
    /// Generate barcode image from text
    /// </summary>
    /// <param name="text">Text to encode</param>
    /// <param name="format">Barcode format (default: QR_CODE)</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <returns>Path to generated barcode image</returns>
    Task<string?> GenerateBarcodeAsync(string text, BarcodeFormat format = BarcodeFormat.QrCode, int width = 300, int height = 300);

    /// <summary>
    /// Validate barcode format
    /// </summary>
    /// <param name="barcode">Barcode text to validate</param>
    /// <param name="expectedFormat">Expected barcode format</param>
    /// <returns>True if valid</returns>
    bool ValidateBarcode(string barcode, BarcodeFormat expectedFormat);

    /// <summary>
    /// Get supported barcode formats
    /// </summary>
    /// <returns>List of supported formats</returns>
    IEnumerable<BarcodeFormat> GetSupportedFormats();

    /// <summary>
    /// Check if camera permission is granted for scanning
    /// </summary>
    /// <returns>True if permission granted</returns>
    Task<bool> HasCameraPermissionAsync();

    /// <summary>
    /// Request camera permission for scanning
    /// </summary>
    /// <returns>True if permission granted</returns>
    Task<bool> RequestCameraPermissionAsync();
}

public class BarcodeResult
{
    public string Value { get; set; } = string.Empty;
    public BarcodeFormat Format { get; set; }
    public DateTime ScannedAt { get; set; } = DateTime.Now;
    public string DisplayValue => $"{Format}: {Value}";
}