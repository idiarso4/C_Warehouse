using WarehouseApp.Core.Models;

namespace WarehouseApp.MAUI.Services;

public interface IRealtimeService
{
    /// <summary>
    /// Connection state
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Current user ID
    /// </summary>
    string? CurrentUserId { get; }

    /// <summary>
    /// Connect to SignalR hub
    /// </summary>
    Task<bool> ConnectAsync(string userId);

    /// <summary>
    /// Disconnect from SignalR hub
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Join a specific group (e.g., warehouse, location)
    /// </summary>
    Task JoinGroupAsync(string groupName);

    /// <summary>
    /// Leave a specific group
    /// </summary>
    Task LeaveGroupAsync(string groupName);

    /// <summary>
    /// Send stock update to other users
    /// </summary>
    Task SendStockUpdateAsync(int productId, int newStock, string locationId, string action);

    /// <summary>
    /// Send notification to specific user or group
    /// </summary>
    Task SendNotificationAsync(string message, NotificationType type, string? targetUserId = null, string? targetGroup = null);

    /// <summary>
    /// Send user activity update
    /// </summary>
    Task SendUserActivityAsync(string activity, string details);

    /// <summary>
    /// Request real-time dashboard data
    /// </summary>
    Task RequestDashboardUpdateAsync();

    /// <summary>
    /// Send location update (for mobile users)
    /// </summary>
    Task SendLocationUpdateAsync(string locationId, string locationName);

    // Events for receiving real-time updates
    event EventHandler<StockUpdateEventArgs>? StockUpdated;
    event EventHandler<NotificationEventArgs>? NotificationReceived;
    event EventHandler<UserActivityEventArgs>? UserActivityUpdated;
    event EventHandler<DashboardUpdateEventArgs>? DashboardUpdated;
    event EventHandler<LocationUpdateEventArgs>? LocationUpdated;
    event EventHandler<ConnectionEventArgs>? ConnectionStateChanged;
    event EventHandler<UserStatusEventArgs>? UserStatusChanged;
}

// Event argument classes
public class StockUpdateEventArgs : EventArgs
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public int OldStock { get; set; }
    public int NewStock { get; set; }
    public int QuantityChanged { get; set; }
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "IN", "OUT", "ADJUST", "TRANSFER"
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Notes { get; set; } = string.Empty;
}

public class NotificationEventArgs : EventArgs
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string FromUserId { get; set; } = string.Empty;
    public string FromUserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public Dictionary<string, object> Data { get; set; } = new();
    public bool IsRead { get; set; } = false;
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
}

public class UserActivityEventArgs : EventArgs
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Activity { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public UserStatus Status { get; set; }
}

public class DashboardUpdateEventArgs : EventArgs
{
    public Dictionary<string, object> Metrics { get; set; } = new();
    public List<RecentActivity> RecentActivities { get; set; } = new();
    public List<StockAlert> StockAlerts { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}

public class LocationUpdateEventArgs : EventArgs
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class ConnectionEventArgs : EventArgs
{
    public bool IsConnected { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class UserStatusEventArgs : EventArgs
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; } = DateTime.Now;
}

// Supporting classes
public class RecentActivity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // "Product", "Location", "User"
    public string EntityId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class StockAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public AlertType AlertType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    StockAlert,
    UserActivity,
    SystemUpdate
}

public enum UserStatus
{
    Online,
    Offline,
    Away,
    Busy,
    InWarehouse
}

public enum AlertType
{
    LowStock,
    OutOfStock,
    OverStock,
    ExpiringStock
}