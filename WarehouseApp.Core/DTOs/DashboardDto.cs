using WarehouseApp.Core.Enums;

namespace WarehouseApp.Core.DTOs;

public class DashboardDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<LowStockAlertDto> LowStockAlerts { get; set; } = new();
    public List<ExpiryAlertDto> ExpiryAlerts { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
}

public class DashboardStatsDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalLocations { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int TodayMovements { get; set; }
    public int ActiveUsers { get; set; }
}

public class RecentActivityDto
{
    public int Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
}

public class LowStockAlertDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public StockStatus Status { get; set; }
}

public class ExpiryAlertDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public int DaysToExpiry { get; set; }
    public int Quantity { get; set; }
    public string LocationName { get; set; } = string.Empty;
}

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int TotalMovements { get; set; }
    public int TotalQuantity { get; set; }
    public decimal Value { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}