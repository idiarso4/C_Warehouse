using WarehouseApp.Core.DTOs;
using WarehouseApp.Core.Enums;

namespace WarehouseApp.Core.Interfaces;

public interface IReportService
{
    // Stock Reports
    Task<ApiResponse<byte[]>> GenerateStockLevelReportAsync(ExportFormat format, int? categoryId = null, int? locationId = null);
    Task<ApiResponse<byte[]>> GenerateLowStockReportAsync(ExportFormat format);
    Task<ApiResponse<byte[]>> GenerateStockMovementReportAsync(ExportFormat format, DateTime? fromDate = null, DateTime? toDate = null, int? productId = null, int? locationId = null);

    // Category Reports
    Task<ApiResponse<byte[]>> GenerateCategoryAnalysisReportAsync(ExportFormat format, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResponse<byte[]>> GenerateProductByCategoryReportAsync(ExportFormat format, int? categoryId = null);

    // Location Reports
    Task<ApiResponse<byte[]>> GenerateLocationUtilizationReportAsync(ExportFormat format);
    Task<ApiResponse<byte[]>> GenerateLocationStockReportAsync(ExportFormat format, int? locationId = null);

    // Supplier Reports
    Task<ApiResponse<byte[]>> GenerateSupplierReportAsync(ExportFormat format, int? supplierId = null);
    Task<ApiResponse<byte[]>> GenerateProductBySupplierReportAsync(ExportFormat format, int? supplierId = null);

    // Movement Reports
    Task<ApiResponse<byte[]>> GenerateMovementSummaryReportAsync(ExportFormat format, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResponse<byte[]>> GenerateUserActivityReportAsync(ExportFormat format, int? userId = null, DateTime? fromDate = null, DateTime? toDate = null);

    // Inventory Reports
    Task<ApiResponse<byte[]>> GenerateInventoryValuationReportAsync(ExportFormat format);
    Task<ApiResponse<byte[]>> GenerateExpiryReportAsync(ExportFormat format, int daysAhead = 30);
    Task<ApiResponse<byte[]>> GenerateABCAnalysisReportAsync(ExportFormat format);

    // Custom Reports
    Task<ApiResponse<byte[]>> GenerateCustomReportAsync(ExportFormat format, string reportQuery, Dictionary<string, object> parameters);

    // Report Templates
    Task<ApiResponse<List<object>>> GetReportTemplatesAsync();
    Task<ApiResponse<object>> GetReportTemplateAsync(int templateId);
    Task<ApiResponse<bool>> SaveReportTemplateAsync(object template);
    Task<ApiResponse<bool>> DeleteReportTemplateAsync(int templateId);

    // Scheduled Reports
    Task<ApiResponse<bool>> ScheduleReportAsync(int templateId, string cronExpression, List<string> emailRecipients);
    Task<ApiResponse<List<object>>> GetScheduledReportsAsync();
    Task<ApiResponse<bool>> CancelScheduledReportAsync(int scheduleId);
}