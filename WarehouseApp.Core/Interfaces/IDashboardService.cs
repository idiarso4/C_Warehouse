using WarehouseApp.Core.DTOs;
using WarehouseApp.Core.Enums;

namespace WarehouseApp.Core.Interfaces;

public interface IDashboardService
{
    // Main Dashboard
    Task<ApiResponse<DashboardDto>> GetDashboardDataAsync();
    Task<ApiResponse<DashboardStatsDto>> GetStatsAsync();

    // Recent Activities
    Task<ApiResponse<List<RecentActivityDto>>> GetRecentActivitiesAsync(int count = 10);
    Task<ApiResponse<List<RecentActivityDto>>> GetUserActivitiesAsync(int userId, int count = 10);

    // Alerts and Notifications
    Task<ApiResponse<List<LowStockAlertDto>>> GetLowStockAlertsAsync(int count = 10);
    Task<ApiResponse<List<ExpiryAlertDto>>> GetExpiryAlertsAsync(int daysAhead = 30, int count = 10);
    Task<ApiResponse<int>> GetTotalAlertsCountAsync();

    // Top Products and Analytics
    Task<ApiResponse<List<TopProductDto>>> GetTopProductsByMovementAsync(int count = 10, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResponse<List<TopProductDto>>> GetTopProductsByValueAsync(int count = 10);
    Task<ApiResponse<List<CategoryDto>>> GetTopCategoriesAsync(int count = 10);

    // Statistics
    Task<ApiResponse<int>> GetTotalProductsAsync();
    Task<ApiResponse<int>> GetTotalCategoriesAsync();
    Task<ApiResponse<int>> GetTotalLocationsAsync();
    Task<ApiResponse<int>> GetActiveUsersCountAsync();
    Task<ApiResponse<decimal>> GetTotalInventoryValueAsync();

    // Movement Statistics
    Task<ApiResponse<int>> GetTodayMovementsCountAsync();
    Task<ApiResponse<int>> GetWeeklyMovementsCountAsync();
    Task<ApiResponse<int>> GetMonthlyMovementsCountAsync();
    Task<ApiResponse<List<(DateTime Date, int Count)>>> GetMovementTrendsAsync(int days = 30);

    // Stock Status
    Task<ApiResponse<int>> GetLowStockCountAsync();
    Task<ApiResponse<int>> GetOutOfStockCountAsync();
    Task<ApiResponse<int>> GetOverstockCountAsync();
    Task<ApiResponse<List<(StockStatus Status, int Count)>>> GetStockStatusSummaryAsync();

    // Performance Metrics
    Task<ApiResponse<decimal>> GetInventoryTurnoverRateAsync();
    Task<ApiResponse<decimal>> GetLocationUtilizationAsync();
    Task<ApiResponse<List<(string LocationName, decimal Utilization)>>> GetTopLocationsByUtilizationAsync(int count = 10);
}