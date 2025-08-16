using WarehouseApp.Core.DTOs;

namespace WarehouseApp.Core.Interfaces;

public interface ICategoryService
{
    // CRUD Operations
    Task<ApiResponse<CategoryDto>> GetByIdAsync(int id);
    Task<ApiResponse<List<CategoryDto>>> GetAllAsync(bool includeInactive = false);
    Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto createDto);
    Task<ApiResponse<CategoryDto>> UpdateAsync(UpdateCategoryDto updateDto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<bool>> ToggleActiveAsync(int id);

    // Hierarchy Management
    Task<ApiResponse<List<CategoryDto>>> GetRootCategoriesAsync();
    Task<ApiResponse<List<CategoryDto>>> GetSubCategoriesAsync(int parentId);
    Task<ApiResponse<List<CategoryDto>>> GetCategoryTreeAsync();
    Task<ApiResponse<bool>> MoveCategoryAsync(int categoryId, int? newParentId);

    // Search and Filter
    Task<ApiResponse<List<CategoryDto>>> SearchAsync(string searchTerm);
    Task<ApiResponse<CategoryDto?>> GetByNameAsync(string name);

    // Validation
    Task<ApiResponse<bool>> IsNameUniqueAsync(string name, int? excludeId = null, int? parentId = null);
    Task<ApiResponse<bool>> CanDeleteAsync(int id);
    Task<ApiResponse<bool>> HasProductsAsync(int id);

    // Sorting
    Task<ApiResponse<bool>> UpdateSortOrderAsync(List<(int CategoryId, int SortOrder)> sortOrders);

    // Statistics
    Task<ApiResponse<int>> GetProductCountAsync(int categoryId, bool includeSubCategories = false);
    Task<ApiResponse<List<CategoryDto>>> GetCategoriesWithProductCountAsync();
}