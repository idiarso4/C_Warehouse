namespace WarehouseApp.Core.Enums;

public enum SyncStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4
}

public enum StockStatus
{
    InStock = 1,
    LowStock = 2,
    OutOfStock = 3,
    Overstock = 4
}

public enum ReportType
{
    StockLevel = 1,
    StockMovement = 2,
    LowStock = 3,
    CategoryAnalysis = 4,
    LocationUtilization = 5,
    SupplierReport = 6
}

public enum ExportFormat
{
    PDF = 1,
    Excel = 2,
    CSV = 3
}

public enum NotificationType
{
    LowStock = 1,
    StockOut = 2,
    ExpiryAlert = 3,
    SystemUpdate = 4,
    UserActivity = 5
}

public enum OperationResult
{
    Success = 1,
    Failed = 2,
    Warning = 3,
    Info = 4
}