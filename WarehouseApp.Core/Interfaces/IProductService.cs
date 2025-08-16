using WarehouseApp.Core.DTOs;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Core.Interfaces;

public interface IProductService
{
    // CRUD Operations
    Task<ApiResponse<ProductDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<ProductDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null, int? categoryId = null, bool? isActive = null);
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto createDto);
    Task<ApiResponse<ProductDto>> UpdateAsync(UpdateProductDto updateDto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<bool>> ToggleActiveAsync(int id);

    // Search and Filter
    Task<ApiResponse<PagedResponse<ProductDto>>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 20);
    Task<ApiResponse<List<ProductDto>>> GetByCategoryAsync(int categoryId);
    Task<ApiResponse<List<ProductDto>>> GetBySupplierAsync(int supplierId);
    Task<ApiResponse<ProductDto?>> GetByBarcodeAsync(string barcode);
    Task<ApiResponse<ProductDto?>> GetBySKUAsync(string sku);

    // Stock Management
    Task<ApiResponse<List<LowStockAlertDto>>> GetLowStockProductsAsync();
    Task<ApiResponse<List<ExpiryAlertDto>>> GetExpiringProductsAsync(int daysAhead = 30);
    Task<ApiResponse<bool>> UpdateStockAsync(int productId, int newStock, string reason, string userId);

    // Validation
    Task<ApiResponse<bool>> IsSKUUniqueAsync(string sku, int? excludeId = null);
    Task<ApiResponse<bool>> IsBarcodeUniqueAsync(string barcode, int? excludeId = null);

    // Bulk Operations
    Task<ApiResponse<bool>> BulkUpdateCategoryAsync(List<int> productIds, int newCategoryId);
    Task<ApiResponse<bool>> BulkUpdateSupplierAsync(List<int> productIds, int newSupplierId);
    Task<ApiResponse<bool>> BulkToggleActiveAsync(List<int> productIds, bool isActive);

    // Image Management
    Task<ApiResponse<string>> UploadImageAsync(int productId, byte[] imageData, string fileName);
    Task<ApiResponse<bool>> DeleteImageAsync(int productId);
}