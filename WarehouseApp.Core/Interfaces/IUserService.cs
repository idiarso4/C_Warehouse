using WarehouseApp.Core.DTOs;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Core.Interfaces;

public interface IUserService
{
    // Authentication
    Task<ApiResponse<UserDto>> AuthenticateAsync(LoginDto loginDto);
    Task<ApiResponse<string>> GenerateTokenAsync(UserDto user);
    Task<ApiResponse<bool>> ValidateTokenAsync(string token);
    Task<ApiResponse<UserDto>> GetUserFromTokenAsync(string token);

    // CRUD Operations
    Task<ApiResponse<UserDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<UserDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 20, string? searchTerm = null, UserRole? role = null, bool? isActive = null);
    Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto createDto);
    Task<ApiResponse<UserDto>> UpdateAsync(UpdateUserDto updateDto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<bool>> ToggleActiveAsync(int id);

    // Password Management
    Task<ApiResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<ApiResponse<bool>> ResetPasswordAsync(int userId, string newPassword);
    Task<ApiResponse<bool>> ValidatePasswordAsync(string password);

    // Search and Filter
    Task<ApiResponse<List<UserDto>>> SearchAsync(string searchTerm);
    Task<ApiResponse<UserDto?>> GetByUsernameAsync(string username);
    Task<ApiResponse<UserDto?>> GetByEmailAsync(string email);
    Task<ApiResponse<List<UserDto>>> GetByRoleAsync(UserRole role);

    // Validation
    Task<ApiResponse<bool>> IsUsernameUniqueAsync(string username, int? excludeId = null);
    Task<ApiResponse<bool>> IsEmailUniqueAsync(string email, int? excludeId = null);

    // Profile Management
    Task<ApiResponse<bool>> UpdateProfileAsync(int userId, string firstName, string lastName, string? phoneNumber);
    Task<ApiResponse<string>> UploadProfileImageAsync(int userId, byte[] imageData, string fileName);
    Task<ApiResponse<bool>> DeleteProfileImageAsync(int userId);

    // Activity Tracking
    Task<ApiResponse<bool>> UpdateLastLoginAsync(int userId);
    Task<ApiResponse<List<RecentActivityDto>>> GetUserActivitiesAsync(int userId, int pageNumber = 1, int pageSize = 20);

    // Role Management
    Task<ApiResponse<bool>> UpdateRoleAsync(int userId, UserRole newRole);
    Task<ApiResponse<List<UserDto>>> GetActiveUsersAsync();
}