using WarehouseApp.Core.DTOs;
using WarehouseApp.Core.Enums;

namespace WarehouseApp.Core.Interfaces;

public interface ISyncService
{
    // Sync Operations
    Task<ApiResponse<SyncResponse>> SyncAllAsync();
    Task<ApiResponse<SyncResponse>> SyncProductsAsync();
    Task<ApiResponse<SyncResponse>> SyncCategoriesAsync();
    Task<ApiResponse<SyncResponse>> SyncLocationsAsync();
    Task<ApiResponse<SyncResponse>> SyncStockMovementsAsync();
    Task<ApiResponse<SyncResponse>> SyncUsersAsync();

    // Sync Status
    Task<ApiResponse<SyncStatus>> GetSyncStatusAsync();
    Task<ApiResponse<DateTime?>> GetLastSyncTimeAsync();
    Task<ApiResponse<bool>> IsSyncInProgressAsync();

    // Conflict Resolution
    Task<ApiResponse<List<object>>> GetConflictsAsync();
    Task<ApiResponse<bool>> ResolveConflictAsync(int conflictId, object resolution);
    Task<ApiResponse<bool>> ResolveAllConflictsAsync(List<(int ConflictId, object Resolution)> resolutions);

    // Change Tracking
    Task<ApiResponse<bool>> TrackChangeAsync(string entityType, int entityId, string operation, object data);
    Task<ApiResponse<List<object>>> GetPendingChangesAsync();
    Task<ApiResponse<bool>> ClearPendingChangesAsync();

    // Background Sync
    Task<ApiResponse<bool>> StartBackgroundSyncAsync();
    Task<ApiResponse<bool>> StopBackgroundSyncAsync();
    Task<ApiResponse<bool>> IsBackgroundSyncEnabledAsync();

    // Sync Configuration
    Task<ApiResponse<bool>> SetSyncIntervalAsync(int minutes);
    Task<ApiResponse<int>> GetSyncIntervalAsync();
    Task<ApiResponse<bool>> EnableAutoSyncAsync(bool enabled);
    Task<ApiResponse<bool>> IsAutoSyncEnabledAsync();

    // Network and Connectivity
    Task<ApiResponse<bool>> CheckConnectivityAsync();
    Task<ApiResponse<bool>> TestApiConnectionAsync();
}