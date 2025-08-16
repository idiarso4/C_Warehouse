using WarehouseApp.Core.DTOs;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Core.Interfaces;

public interface IStockService
{
    // Stock Movement Operations
    Task<ApiResponse<StockMovementDto>> CreateMovementAsync(CreateStockMovementDto createDto, string userId);
    Task<ApiResponse<PagedResponse<StockMovementDto>>> GetMovementsAsync(int pageNumber = 1, int pageSize = 20, int? productId = null, int? locationId = null, MovementType? movementType = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResponse<StockMovementDto>> GetMovementByIdAsync(int id);

    // Stock In Operations
    Task<ApiResponse<bool>> StockInAsync(int productId, int locationId, int quantity, string? reference, string? notes, string userId);
    Task<ApiResponse<bool>> BulkStockInAsync(List<(int ProductId, int LocationId, int Quantity, string? Reference, string? Notes)> items, string userId);

    // Stock Out Operations
    Task<ApiResponse<bool>> StockOutAsync(int productId, int locationId, int quantity, string? reference, string? notes, string userId);
    Task<ApiResponse<bool>> BulkStockOutAsync(List<(int ProductId, int LocationId, int Quantity, string? Reference, string? Notes)> items, string userId);

    // Transfer Operations
    Task<ApiResponse<bool>> TransferStockAsync(int productId, int fromLocationId, int toLocationId, int quantity, string? reference, string? notes, string userId);
    Task<ApiResponse<bool>> BulkTransferAsync(List<(int ProductId, int FromLocationId, int ToLocationId, int Quantity, string? Reference, string? Notes)> items, string userId);

    // Stock Adjustment
    Task<ApiResponse<bool>> AdjustStockAsync(int productId, int locationId, int newQuantity, string reason, string userId);

    // Stock Queries
    Task<ApiResponse<int>> GetCurrentStockAsync(int productId, int? locationId = null);
    Task<ApiResponse<List<ProductLocationDto>>> GetProductLocationsAsync(int productId);
    Task<ApiResponse<List<ProductDto>>> GetProductsByLocationAsync(int locationId);

    // Stock Validation
    Task<ApiResponse<bool>> ValidateStockAvailabilityAsync(int productId, int locationId, int requiredQuantity);
    Task<ApiResponse<bool>> CanPerformMovementAsync(int productId, int locationId, MovementType movementType, int quantity);

    // Reports
    Task<ApiResponse<List<StockMovementDto>>> GetMovementHistoryAsync(int productId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResponse<List<StockMovementDto>>> GetLocationMovementsAsync(int locationId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ApiResponse<List<StockMovementDto>>> GetDailyMovementsAsync(DateTime date);
}