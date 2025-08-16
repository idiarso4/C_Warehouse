using WarehouseApp.Core.DTOs;

namespace WarehouseApp.Core.Interfaces;

public interface ILocationService
{
    // CRUD Operations
    Task<ApiResponse<LocationDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<LocationDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null, string? zone = null, bool? isActive = null);
    Task<ApiResponse<LocationDto>> CreateAsync(CreateLocationDto createDto);
    Task<ApiResponse<LocationDto>> UpdateAsync(UpdateLocationDto updateDto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<bool>> ToggleActiveAsync(int id);

    // Search and Filter
    Task<ApiResponse<List<LocationDto>>> SearchAsync(string searchTerm);
    Task<ApiResponse<LocationDto?>> GetByCodeAsync(string code);
    Task<ApiResponse<LocationDto?>> GetByQRCodeAsync(string qrCode);
    Task<ApiResponse<List<LocationDto>>> GetByZoneAsync(string zone);

    // Hierarchy and Organization
    Task<ApiResponse<List<string>>> GetZonesAsync();
    Task<ApiResponse<List<string>>> GetAislesAsync(string? zone = null);
    Task<ApiResponse<List<LocationDto>>> GetLocationsByHierarchyAsync(string? zone = null, string? aisle = null, string? shelf = null);

    // Validation
    Task<ApiResponse<bool>> IsCodeUniqueAsync(string code, int? excludeId = null);
    Task<ApiResponse<bool>> CanDeleteAsync(int id);
    Task<ApiResponse<bool>> HasProductsAsync(int id);

    // QR Code Management
    Task<ApiResponse<string>> GenerateQRCodeAsync(int locationId);
    Task<ApiResponse<bool>> UpdateQRCodeAsync(int locationId, string qrCode);

    // Capacity Management
    Task<ApiResponse<bool>> UpdateCapacityAsync(int locationId, decimal currentCapacity);
    Task<ApiResponse<List<LocationDto>>> GetOverCapacityLocationsAsync();
    Task<ApiResponse<decimal>> GetCapacityUtilizationAsync(int locationId);

    // Statistics and Reports
    Task<ApiResponse<int>> GetProductCountAsync(int locationId);
    Task<ApiResponse<List<LocationDto>>> GetLocationsWithProductCountAsync();
    Task<ApiResponse<List<LocationDto>>> GetEmptyLocationsAsync();
}