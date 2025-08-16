using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WarehouseApp.MAUI.Services;

public class RealtimeService : IRealtimeService, IDisposable
{
    private readonly ILogger<RealtimeService> _logger;
    private HubConnection? _hubConnection;
    private readonly string _hubUrl;
    private bool _disposed = false;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    public string? CurrentUserId { get; private set; }

    // Events
    public event EventHandler<StockUpdateEventArgs>? StockUpdated;
    public event EventHandler<NotificationEventArgs>? NotificationReceived;
    public event EventHandler<UserActivityEventArgs>? UserActivityUpdated;
    public event EventHandler<DashboardUpdateEventArgs>? DashboardUpdated;
    public event EventHandler<LocationUpdateEventArgs>? LocationUpdated;
    public event EventHandler<ConnectionEventArgs>? ConnectionStateChanged;
    public event EventHandler<UserStatusEventArgs>? UserStatusChanged;

    public RealtimeService(ILogger<RealtimeService> logger)
    {
        _logger = logger;
        // TODO: Get from configuration
        _hubUrl = "https://localhost:7001/warehousehub"; // This would be your server URL
    }

    public async Task<bool> ConnectAsync(string userId)
    {
        try
        {
            if (IsConnected)
            {
                _logger.LogInformation("Already connected to SignalR hub");
                return true;
            }

            CurrentUserId = userId;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options =>
                {
                    // Add authentication headers if needed
                    // options.Headers.Add("Authorization", $"Bearer {token}");
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
                .Build();

            // Setup event handlers
            SetupEventHandlers();

            // Setup connection state handlers
            _hubConnection.Closed += OnConnectionClosed;
            _hubConnection.Reconnecting += OnReconnecting;
            _hubConnection.Reconnected += OnReconnected;

            await _hubConnection.StartAsync();

            // Register user with the hub
            await _hubConnection.InvokeAsync("RegisterUser", userId);

            _logger.LogInformation($"Connected to SignalR hub as user: {userId}");

            ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs { IsConnected = true });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to connect to SignalR hub: {ex.Message}");

            ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs
            {
                IsConnected = false,
                ErrorMessage = ex.Message
            });

            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            if (_hubConnection != null)
            {
                if (IsConnected && !string.IsNullOrEmpty(CurrentUserId))
                {
                    await _hubConnection.InvokeAsync("UnregisterUser", CurrentUserId);
                }

                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }

            CurrentUserId = null;
            _logger.LogInformation("Disconnected from SignalR hub");

            ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs { IsConnected = false });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error disconnecting from SignalR hub: {ex.Message}");
        }
    }

    public async Task JoinGroupAsync(string groupName)
    {
        try
        {
            if (!IsConnected || _hubConnection == null)
            {
                _logger.LogWarning("Cannot join group - not connected to hub");
                return;
            }

            await _hubConnection.InvokeAsync("JoinGroup", groupName);
            _logger.LogInformation($"Joined group: {groupName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error joining group {groupName}: {ex.Message}");
        }
    }

    public async Task LeaveGroupAsync(string groupName)
    {
        try
        {
            if (!IsConnected || _hubConnection == null)
            {
                _logger.LogWarning("Cannot leave group - not connected to hub");
                return;
            }

            await _hubConnection.InvokeAsync("LeaveGroup", groupName);
            _logger.LogInformation($"Left group: {groupName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error leaving group {groupName}: {ex.Message}");
        }
    }

    public async Task SendStockUpdateAsync(int productId, int newStock, string locationId, string action)
    {
        try
        {
            if (!IsConnected || _hubConnection == null)
            {
                _logger.LogWarning("Cannot send stock update - not connected to hub");
                return;
            }

            var stockUpdate = new
            {
                ProductId = productId,
                NewStock = newStock,
                LocationId = locationId,
                Action = action,
                UserId = CurrentUserId,
                Timestamp = DateTime.Now
            };

            await _hubConnection.InvokeAsync("SendStockUpdate", stockUpdate);
            _logger.LogInformation($"Sent stock update for product {productId}: {action} -> {newStock}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending stock update: {ex.Message}");
        }
    }

    public async Task SendNotificationAsync(string message, NotificationType type, string? targetUserId = null, string? targetGroup = null)
    {
        try
        {
            if (!IsConnected || _hubConnection == null)
            {
                _logger.LogWarning("Cannot send notification - not connected to hub");
                return;
            }

            var notification = new
            {
                Message = message,
                Type = type.ToString(),
                FromUserId = CurrentUserId,
                TargetUserId = targetUserId,
                TargetGroup = targetGroup,
                Timestamp = DateTime.Now
            };

            await _hubConnection.InvokeAsync("SendNotification", notification);
            _logger.LogInformation($"Sent notification: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending notification: {ex.Message}");
        }
    }

    public async Task SendUserActivityAsync(string activity, string details)
    {
        try
        {
            if (!IsConnected || _hubConnection == null)
            {
                _logger.LogWarning("Cannot send user activity - not connected to hub");
                return;
            }

            var userActivity = new
            {
                UserId = CurrentUserId,
                Activity = activity,
                Details = details,
                Timestamp = DateTime.Now
            };

            await _hubConnection.InvokeAsync("SendUserActivity", userActivity);
            _logger.LogDebug($"Sent user activity: {activity}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending user activity: {ex.Message}");
        }
    }

    public async Task RequestDashboardUpdateAsync()
    {
        try
        {
            if (!IsConnected || _hubConnection == null)
            {
                _logger.LogWarning("Cannot request dashboard update - not connected to hub");
                return;
            }

            await _hubConnection.InvokeAsync("RequestDashboardUpdate");
            _logger.LogDebug("Requested dashboard update");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error requesting dashboard update: {ex.Message}");
        }
    }

    public async Task SendLocationUpdateAsync(string locationId, string locationName)
    {
        try
        {
            if (!IsConnected || _hubConnection == null)
            {
                _logger.LogWarning("Cannot send location update - not connected to hub");
                return;
            }

            var locationUpdate = new
            {
                UserId = CurrentUserId,
                LocationId = locationId,
                LocationName = locationName,
                Timestamp = DateTime.Now
            };

            await _hubConnection.InvokeAsync("SendLocationUpdate", locationUpdate);
            _logger.LogDebug($"Sent location update: {locationName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending location update: {ex.Message}");
        }
    }

    private void SetupEventHandlers()
    {
        if (_hubConnection == null) return;

        // Stock update events
        _hubConnection.On<string>("ReceiveStockUpdate", (jsonData) =>
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
                var stockUpdate = new StockUpdateEventArgs
                {
                    ProductId = data.GetProperty("productId").GetInt32(),
                    ProductName = data.TryGetProperty("productName", out var pName) ? pName.GetString() ?? "" : "",
                    ProductSKU = data.TryGetProperty("productSKU", out var pSKU) ? pSKU.GetString() ?? "" : "",
                    OldStock = data.TryGetProperty("oldStock", out var oStock) ? oStock.GetInt32() : 0,
                    NewStock = data.GetProperty("newStock").GetInt32(),
                    QuantityChanged = data.TryGetProperty("quantityChanged", out var qChanged) ? qChanged.GetInt32() : 0,
                    LocationId = data.TryGetProperty("locationId", out var lId) ? lId.GetString() ?? "" : "",
                    LocationName = data.TryGetProperty("locationName", out var lName) ? lName.GetString() ?? "" : "",
                    Action = data.TryGetProperty("action", out var action) ? action.GetString() ?? "" : "",
                    UserId = data.TryGetProperty("userId", out var uId) ? uId.GetString() ?? "" : "",
                    UserName = data.TryGetProperty("userName", out var uName) ? uName.GetString() ?? "" : "",
                    Timestamp = data.TryGetProperty("timestamp", out var ts) ? ts.GetDateTime() : DateTime.Now,
                    Notes = data.TryGetProperty("notes", out var notes) ? notes.GetString() ?? "" : ""
                };

                StockUpdated?.Invoke(this, stockUpdate);
                _logger.LogDebug($"Received stock update for product {stockUpdate.ProductId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock update");
            }
        });

        // Notification events
        _hubConnection.On<string>("ReceiveNotification", (jsonData) =>
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
                var notification = new NotificationEventArgs
                {
                    Id = data.TryGetProperty("id", out var id) ? id.GetString() ?? Guid.NewGuid().ToString() : Guid.NewGuid().ToString(),
                    Title = data.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                    Message = data.GetProperty("message").GetString() ?? "",
                    Type = Enum.TryParse<NotificationType>(data.TryGetProperty("type", out var type) ? type.GetString() : "Info", out var notType) ? notType : NotificationType.Info,
                    FromUserId = data.TryGetProperty("fromUserId", out var fromId) ? fromId.GetString() ?? "" : "",
                    FromUserName = data.TryGetProperty("fromUserName", out var fromName) ? fromName.GetString() ?? "" : "",
                    Timestamp = data.TryGetProperty("timestamp", out var ts) ? ts.GetDateTime() : DateTime.Now,
                    ActionUrl = data.TryGetProperty("actionUrl", out var actionUrl) ? actionUrl.GetString() : null,
                    ActionText = data.TryGetProperty("actionText", out var actionText) ? actionText.GetString() : null
                };

                // Parse data dictionary if present
                if (data.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in dataElement.EnumerateObject())
                    {
                        notification.Data[prop.Name] = prop.Value.ToString();
                    }
                }

                NotificationReceived?.Invoke(this, notification);
                _logger.LogDebug($"Received notification: {notification.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification");
            }
        });

        // User activity events
        _hubConnection.On<string>("ReceiveUserActivity", (jsonData) =>
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
                var userActivity = new UserActivityEventArgs
                {
                    UserId = data.TryGetProperty("userId", out var uId) ? uId.GetString() ?? "" : "",
                    UserName = data.TryGetProperty("userName", out var uName) ? uName.GetString() ?? "" : "",
                    Activity = data.TryGetProperty("activity", out var activity) ? activity.GetString() ?? "" : "",
                    Details = data.TryGetProperty("details", out var details) ? details.GetString() ?? "" : "",
                    LocationId = data.TryGetProperty("locationId", out var lId) ? lId.GetString() ?? "" : "",
                    LocationName = data.TryGetProperty("locationName", out var lName) ? lName.GetString() ?? "" : "",
                    Timestamp = data.TryGetProperty("timestamp", out var ts) ? ts.GetDateTime() : DateTime.Now,
                    Status = Enum.TryParse<UserStatus>(data.TryGetProperty("status", out var status) ? status.GetString() : "Online", out var userStatus) ? userStatus : UserStatus.Online
                };

                UserActivityUpdated?.Invoke(this, userActivity);
                _logger.LogDebug($"Received user activity from {userActivity.UserName}: {userActivity.Activity}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing user activity");
            }
        });

        // Dashboard update events
        _hubConnection.On<string>("ReceiveDashboardUpdate", (jsonData) =>
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
                var dashboardUpdate = new DashboardUpdateEventArgs
                {
                    LastUpdated = data.TryGetProperty("lastUpdated", out var lastUpdated) ? lastUpdated.GetDateTime() : DateTime.Now
                };

                // Parse metrics
                if (data.TryGetProperty("metrics", out var metricsElement) && metricsElement.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in metricsElement.EnumerateObject())
                    {
                        dashboardUpdate.Metrics[prop.Name] = prop.Value.ToString();
                    }
                }

                // Parse recent activities
                if (data.TryGetProperty("recentActivities", out var activitiesElement) && activitiesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var activityElement in activitiesElement.EnumerateArray())
                    {
                        var activity = new RecentActivity
                        {
                            Id = activityElement.TryGetProperty("id", out var id) ? id.GetString() ?? Guid.NewGuid().ToString() : Guid.NewGuid().ToString(),
                            UserId = activityElement.TryGetProperty("userId", out var uId) ? uId.GetString() ?? "" : "",
                            UserName = activityElement.TryGetProperty("userName", out var uName) ? uName.GetString() ?? "" : "",
                            Action = activityElement.TryGetProperty("action", out var action) ? action.GetString() ?? "" : "",
                            Details = activityElement.TryGetProperty("details", out var details) ? details.GetString() ?? "" : "",
                            EntityType = activityElement.TryGetProperty("entityType", out var entityType) ? entityType.GetString() ?? "" : "",
                            EntityId = activityElement.TryGetProperty("entityId", out var entityId) ? entityId.GetString() ?? "" : "",
                            Timestamp = activityElement.TryGetProperty("timestamp", out var ts) ? ts.GetDateTime() : DateTime.Now
                        };
                        dashboardUpdate.RecentActivities.Add(activity);
                    }
                }

                // Parse stock alerts
                if (data.TryGetProperty("stockAlerts", out var alertsElement) && alertsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var alertElement in alertsElement.EnumerateArray())
                    {
                        var alert = new StockAlert
                        {
                            Id = alertElement.TryGetProperty("id", out var id) ? id.GetString() ?? Guid.NewGuid().ToString() : Guid.NewGuid().ToString(),
                            ProductId = alertElement.TryGetProperty("productId", out var pId) ? pId.GetInt32() : 0,
                            ProductName = alertElement.TryGetProperty("productName", out var pName) ? pName.GetString() ?? "" : "",
                            ProductSKU = alertElement.TryGetProperty("productSKU", out var pSKU) ? pSKU.GetString() ?? "" : "",
                            CurrentStock = alertElement.TryGetProperty("currentStock", out var cStock) ? cStock.GetInt32() : 0,
                            MinimumStock = alertElement.TryGetProperty("minimumStock", out var mStock) ? mStock.GetInt32() : 0,
                            LocationId = alertElement.TryGetProperty("locationId", out var lId) ? lId.GetString() ?? "" : "",
                            LocationName = alertElement.TryGetProperty("locationName", out var lName) ? lName.GetString() ?? "" : "",
                            AlertType = Enum.TryParse<AlertType>(alertElement.TryGetProperty("alertType", out var alertType) ? alertType.GetString() : "LowStock", out var aType) ? aType : AlertType.LowStock,
                            CreatedAt = alertElement.TryGetProperty("createdAt", out var createdAt) ? createdAt.GetDateTime() : DateTime.Now
                        };
                        dashboardUpdate.StockAlerts.Add(alert);
                    }
                }

                DashboardUpdated?.Invoke(this, dashboardUpdate);
                _logger.LogDebug("Received dashboard update");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing dashboard update");
            }
        });

        // Location update events
        _hubConnection.On<string>("ReceiveLocationUpdate", (jsonData) =>
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
                var locationUpdate = new LocationUpdateEventArgs
                {
                    UserId = data.TryGetProperty("userId", out var uId) ? uId.GetString() ?? "" : "",
                    UserName = data.TryGetProperty("userName", out var uName) ? uName.GetString() ?? "" : "",
                    LocationId = data.TryGetProperty("locationId", out var lId) ? lId.GetString() ?? "" : "",
                    LocationName = data.TryGetProperty("locationName", out var lName) ? lName.GetString() ?? "" : "",
                    Timestamp = data.TryGetProperty("timestamp", out var ts) ? ts.GetDateTime() : DateTime.Now
                };

                LocationUpdated?.Invoke(this, locationUpdate);
                _logger.LogDebug($"Received location update from {locationUpdate.UserName}: {locationUpdate.LocationName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing location update");
            }
        });

        // User status events
        _hubConnection.On<string>("ReceiveUserStatus", (jsonData) =>
        {
            try
            {
                var data = JsonSerializer.Deserialize<JsonElement>(jsonData);
                var userStatus = new UserStatusEventArgs
                {
                    UserId = data.TryGetProperty("userId", out var uId) ? uId.GetString() ?? "" : "",
                    UserName = data.TryGetProperty("userName", out var uName) ? uName.GetString() ?? "" : "",
                    Status = Enum.TryParse<UserStatus>(data.TryGetProperty("status", out var status) ? status.GetString() : "Online", out var uStatus) ? uStatus : UserStatus.Online,
                    LocationId = data.TryGetProperty("locationId", out var lId) ? lId.GetString() ?? "" : "",
                    LocationName = data.TryGetProperty("locationName", out var lName) ? lName.GetString() ?? "" : "",
                    LastSeen = data.TryGetProperty("lastSeen", out var lastSeen) ? lastSeen.GetDateTime() : DateTime.Now
                };

                UserStatusChanged?.Invoke(this, userStatus);
                _logger.LogDebug($"Received user status update: {userStatus.UserName} is {userStatus.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing user status");
            }
        });
    }

    private async Task OnConnectionClosed(Exception? exception)
    {
        _logger.LogWarning($"SignalR connection closed. Exception: {exception?.Message}");

        ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs
        {
            IsConnected = false,
            ErrorMessage = exception?.Message
        });

        // Attempt to reconnect after a delay if not disposed
        if (!_disposed)
        {
            await Task.Delay(5000); // Wait 5 seconds before attempting to reconnect

            if (!_disposed && !string.IsNullOrEmpty(CurrentUserId))
            {
                _logger.LogInformation("Attempting to reconnect to SignalR hub...");
                await ConnectAsync(CurrentUserId);
            }
        }
    }

    private async Task OnReconnecting(Exception? exception)
    {
        _logger.LogInformation($"SignalR reconnecting. Exception: {exception?.Message}");

        ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs
        {
            IsConnected = false,
            ErrorMessage = "Reconnecting..."
        });

        await Task.CompletedTask;
    }

    private async Task OnReconnected(string? connectionId)
    {
        _logger.LogInformation($"SignalR reconnected with connection ID: {connectionId}");

        ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs { IsConnected = true });

        // Re-register user after reconnection
        if (!string.IsNullOrEmpty(CurrentUserId) && _hubConnection != null)
        {
            try
            {
                await _hubConnection.InvokeAsync("RegisterUser", CurrentUserId);
                _logger.LogInformation($"Re-registered user after reconnection: {CurrentUserId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error re-registering user after reconnection");
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;

            try
            {
                // Disconnect synchronously on dispose
                if (_hubConnection != null)
                {
                    if (IsConnected && !string.IsNullOrEmpty(CurrentUserId))
                    {
                        // Fire and forget - don't await on dispose
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await _hubConnection.InvokeAsync("UnregisterUser", CurrentUserId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error unregistering user during dispose");
                            }
                        });
                    }

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _hubConnection.DisposeAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error disposing hub connection");
                        }
                    });
                }

                // Clear event handlers
                StockUpdated = null;
                NotificationReceived = null;
                UserActivityUpdated = null;
                DashboardUpdated = null;
                LocationUpdated = null;
                ConnectionStateChanged = null;
                UserStatusChanged = null;

                _logger.LogInformation("RealtimeService disposed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during RealtimeService disposal");
            }
        }
    }

    ~RealtimeService()
    {
        Dispose(false);
    }
}