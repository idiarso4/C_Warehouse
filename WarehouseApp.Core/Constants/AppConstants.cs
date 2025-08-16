namespace WarehouseApp.Core.Constants;

public static class AppConstants
{
    public static class Database
    {
        public const string DatabaseName = "WarehouseApp.db";
        public const int CurrentVersion = 1;
    }

    public static class Sync
    {
        public const int SyncIntervalMinutes = 15;
        public const int MaxRetryAttempts = 3;
        public const int TimeoutSeconds = 30;
    }

    public static class Validation
    {
        public const int MinPasswordLength = 6;
        public const int MaxFileSize = 5 * 1024 * 1024; // 5MB
        public const int MaxImageSize = 2 * 1024 * 1024; // 2MB
    }

    public static class UI
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;
        public const int SearchDelayMs = 500;
    }

    public static class Storage
    {
        public const string ImagesFolder = "Images";
        public const string DocumentsFolder = "Documents";
        public const string BackupFolder = "Backups";
    }

    public static class Permissions
    {
        public const string Camera = "android.permission.CAMERA";
        public const string Storage = "android.permission.WRITE_EXTERNAL_STORAGE";
        public const string Internet = "android.permission.INTERNET";
    }

    public static class DefaultValues
    {
        public const string DefaultCurrency = "IDR";
        public const string DefaultUnit = "pcs";
        public const string DefaultLanguage = "id-ID";
        public const int DefaultLowStockThreshold = 10;
    }

    public static class ApiEndpoints
    {
        public const string Products = "/api/products";
        public const string Categories = "/api/categories";
        public const string Locations = "/api/locations";
        public const string StockMovements = "/api/stock-movements";
        public const string Users = "/api/users";
        public const string Auth = "/api/auth";
        public const string Sync = "/api/sync";
    }
}