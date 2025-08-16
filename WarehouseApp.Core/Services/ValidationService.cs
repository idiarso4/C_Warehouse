using System.Text.RegularExpressions;
using WarehouseApp.Core.Constants;

namespace WarehouseApp.Core.Services;

public static class ValidationService
{
    // Product Validation
    public static bool IsValidSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return false;

        // SKU should be alphanumeric, can contain hyphens and underscores
        var regex = new Regex(@"^[A-Za-z0-9\-_]+$");
        return regex.IsMatch(sku) && sku.Length <= 50;
    }

    public static bool IsValidBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return true; // Barcode is optional

        // Basic barcode validation - numeric, length between 8-18
        var regex = new Regex(@"^\d{8,18}$");
        return regex.IsMatch(barcode);
    }

    public static bool IsValidPrice(decimal price)
    {
        return price >= 0 && price <= 999999999.99m;
    }

    public static bool IsValidStock(int stock)
    {
        return stock >= 0 && stock <= int.MaxValue;
    }

    // Location Validation
    public static bool IsValidLocationCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        // Location code format: Zone-Aisle-Shelf-Position (e.g., A1-B2-C3-D4)
        var regex = new Regex(@"^[A-Za-z0-9]+(-[A-Za-z0-9]+)*$");
        return regex.IsMatch(code) && code.Length <= 50;
    }

    public static bool IsValidQRCode(string qrCode)
    {
        if (string.IsNullOrWhiteSpace(qrCode))
            return true; // QR code is optional

        return qrCode.Length <= 100;
    }

    // User Validation
    public static bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        // Username: alphanumeric, can contain dots, hyphens, underscores
        var regex = new Regex(@"^[A-Za-z0-9\.\-_]+$");
        return regex.IsMatch(username) && username.Length >= 3 && username.Length <= 100;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email) && email.Length <= 100;
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        return password.Length >= AppConstants.Validation.MinPasswordLength;
    }

    public static bool IsValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return true; // Phone number is optional

        // Basic phone number validation
        var regex = new Regex(@"^[\+]?[0-9\-\(\)\s]+$");
        return regex.IsMatch(phoneNumber) && phoneNumber.Length <= 20;
    }

    // Category Validation
    public static bool IsValidCategoryName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return name.Trim().Length >= 2 && name.Length <= 100;
    }

    public static bool IsValidColorCode(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return false;

        // Hex color code validation
        var regex = new Regex(@"^#[0-9A-Fa-f]{6}$");
        return regex.IsMatch(color);
    }

    // General Validation
    public static bool IsValidId(int id)
    {
        return id > 0;
    }

    public static bool IsValidPageNumber(int pageNumber)
    {
        return pageNumber > 0;
    }

    public static bool IsValidPageSize(int pageSize)
    {
        return pageSize > 0 && pageSize <= AppConstants.UI.MaxPageSize;
    }

    public static bool IsValidDateRange(DateTime? fromDate, DateTime? toDate)
    {
        if (fromDate.HasValue && toDate.HasValue)
        {
            return fromDate.Value <= toDate.Value;
        }
        return true;
    }

    // File Validation
    public static bool IsValidImageFile(byte[] imageData, string fileName)
    {
        if (imageData == null || imageData.Length == 0)
            return false;

        if (imageData.Length > AppConstants.Validation.MaxImageSize)
            return false;

        var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

        return validExtensions.Contains(extension);
    }

    public static bool IsValidFileSize(byte[] fileData)
    {
        return fileData != null && fileData.Length > 0 && fileData.Length <= AppConstants.Validation.MaxFileSize;
    }
}