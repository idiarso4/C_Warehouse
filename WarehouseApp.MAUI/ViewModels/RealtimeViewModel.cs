using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using WarehouseApp.MAUI.Services;

namespace WarehouseApp.MAUI.ViewModels;

public partial class RealtimeViewModel : BaseViewModel, IDisposable
{
    private readonly IRealtimeService _realtimeService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<RealtimeViewModel> _logger;
    private bool _disposed = false;

    [ObservableProperty]
    private bool _isConnected = false;

    [ObservableProperty]
    private string _connectionStatus = "Disconnected";

    [ObservableProperty]
    private string _currentUserId = string.Empty;

    [ObservableProperty]
    private string _currentUserName = string.Empty;

    [ObservableProperty]
    private UserStatus _currentUserStatus = UserStatus.Offline;

    [ObservableProperty]
    private string _currentLocationId = string.Empty;

    [ObservableProperty]
    private string _currentLocationName = string.Empty;

    // Real-time notifications
    [ObservableProperty]
    private ObservableCollection<NotificationEventArgs> _notifications = new();

    [ObservableProperty]
    private int _unreadNotificationCount = 0;

    [ObservableProperty]
    private bool _showNotifications = false;

    // Real-time stock updates
    [ObservableProperty]
    private ObservableCollection<StockUpdateEventArgs> _recentStockUpdates = new();

    [ObservableProperty]
    private bool _showStockUpdates = false;

    // User activities
    [ObservableProperty]
    private ObservableCollection<UserActivityEventArgs> _userActivities = new();

    [ObservableProperty]
    private bool _showUserActivities = false;

    // Online users
    [ObservableProperty]
    private ObservableCollection<UserStatusEventArgs> _onlineUsers = new();

    [ObservableProperty]
    private int _onlineUserCount = 0;

    // Dashboard metrics
    [ObservableProperty]
    private Dictionary<string, object> _dashboardMetrics = new();

    [ObservableProperty]
    private ObservableCollection<StockAlert> _stockAlerts = new();

    [ObservableProperty]
    private DateTime _lastDashboardUpdate = DateTime.Now;

    // Connection settings
    [ObservableProperty]
    private bool _autoConnect = true;

    [ObservableProperty]
    private bool _enableNotifications = true;

    [ObservableProperty]
    private bool _enableStockUpdates = true;

    [ObservableProperty]
    private bool _enableUserActivities = true;

    [ObservableProperty]
    private string _selectedGroup = "warehouse-main";

    [ObservableProperty]
    private ObservableCollection<string> _availableGroups = new()
    {
        "warehouse-main",
        "warehouse-north",
        "warehouse-south",
        "managers",
        "staff"
    };

    public RealtimeViewModel(
        IRealtimeService realtimeService,
        INavigationService navigationService,
        ILogger<RealtimeViewModel> logger)
    {
        _realtimeService = realtimeService;
        _navigationService = navigationService;
        _logger = logger;

        Title = "Real-time Dashboard";

        // Subscribe to real-time events
        SubscribeToRealtimeEvents();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Auto-connect if enabled
        if (AutoConnect && !string.IsNullOrEmpty(CurrentUserId))
        {
            await ConnectAsync();
        }
    }

    [RelayCommand]
    private async Task ConnectAsync()
    {
        if (IsBusy || IsConnected) return;

        await SetBusyAsync(async () =>
        {
            try
            {
                // Use current user ID or default for demo
                var userId = !string.IsNullOrEmpty(CurrentUserId) ? CurrentUserId : "demo-user-" + DateTime.Now.Ticks;
                CurrentUserId = userId;
                CurrentUserName = $"User {userId.Substring(userId.Length - 4)}";

                var connected = await _realtimeService.ConnectAsync(userId);

                if (connected)
                {
                    IsConnected = true;
                    ConnectionStatus = "Connected";
                    CurrentUserStatus = UserStatus.Online;

                    // Join default group
                    await _realtimeService.JoinGroupAsync(SelectedGroup);

                    // Send initial user activity
                    await _realtimeService.SendUserActivityAsync("Connected", "User connected to real-time system");

                    // Request dashboard update
                    await _realtimeService.RequestDashboardUpdateAsync();

                    await ShowSuccessAsync("Connected to real-time system!");
                }
                else
                {
                    ConnectionStatus = "Connection failed";
                    await ShowErrorAsync("Failed to connect to real-time system.");
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
                await ShowErrorAsync($"Connection error: {ex.Message}");
            }
        }, "Connecting...");
    }

    [RelayCommand]
    private async Task DisconnectAsync()
    {
        if (IsBusy || !IsConnected) return;

        await SetBusyAsync(async () =>
        {
            try
            {
                await _realtimeService.DisconnectAsync();

                IsConnected = false;
                ConnectionStatus = "Disconnected";
                CurrentUserStatus = UserStatus.Offline;

                // Clear real-time data
                OnlineUsers.Clear();
                OnlineUserCount = 0;

                await ShowInfoAsync("Disconnected from real-time system.");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Disconnect error: {ex.Message}");
            }
        }, "Disconnecting...");
    }

    [RelayCommand]
    private async Task JoinGroupAsync(string groupName)
    {
        if (!IsConnected || string.IsNullOrEmpty(groupName)) return;

        try
        {
            // Leave current group
            if (!string.IsNullOrEmpty(SelectedGroup))
            {
                await _realtimeService.LeaveGroupAsync(SelectedGroup);
            }

            // Join new group
            await _realtimeService.JoinGroupAsync(groupName);
            SelectedGroup = groupName;

            await ShowSuccessAsync($"Joined group: {groupName}");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error joining group: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SendTestNotificationAsync()
    {
        if (!IsConnected) return;

        try
        {
            var message = $"Test notification from {CurrentUserName} at {DateTime.Now:HH:mm:ss}";
            await _realtimeService.SendNotificationAsync(message, NotificationType.Info, targetGroup: SelectedGroup);

            await ShowSuccessAsync("Test notification sent!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error sending notification: {ex.Message}");
        }
    }

    public async Task SendStockUpdateAsync(int productId, int newStock, string action)
    {
        if (!IsConnected) return;

        try
        {
            await _realtimeService.SendStockUpdateAsync(productId, newStock, CurrentLocationId, action);
            await ShowSuccessAsync($"Stock update sent for product {productId}");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error sending stock update: {ex.Message}");
        }
    }

    public async Task UpdateLocationAsync(string locationId, string locationName)
    {
        if (!IsConnected) return;

        try
        {
            CurrentLocationId = locationId;
            CurrentLocationName = locationName;

            await _realtimeService.SendLocationUpdateAsync(locationId, locationName);
            await ShowSuccessAsync($"Location updated to: {locationName}");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error updating location: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task MarkNotificationAsReadAsync(NotificationEventArgs notification)
    {
        if (notification == null) return;

        notification.IsRead = true;
        UpdateUnreadNotificationCount();

        // Navigate to action URL if available
        if (!string.IsNullOrEmpty(notification.ActionUrl))
        {
            await _navigationService.GoToAsync(notification.ActionUrl, notification.Data);
        }
    }

    [RelayCommand]
    private async Task ClearNotificationsAsync()
    {
        var confirmed = await ShowConfirmAsync(
            "Clear Notifications",
            "Are you sure you want to clear all notifications?");

        if (!confirmed) return;

        Notifications.Clear();
        UnreadNotificationCount = 0;
    }

    [RelayCommand]
    private void ToggleNotifications()
    {
        ShowNotifications = !ShowNotifications;
        if (ShowNotifications)
        {
            ShowStockUpdates = false;
            ShowUserActivities = false;
        }
    }

    [RelayCommand]
    private void ToggleStockUpdates()
    {
        ShowStockUpdates = !ShowStockUpdates;
        if (ShowStockUpdates)
        {
            ShowNotifications = false;
            ShowUserActivities = false;
        }
    }

    [RelayCommand]
    private void ToggleUserActivities()
    {
        ShowUserActivities = !ShowUserActivities;
        if (ShowUserActivities)
        {
            ShowNotifications = false;
            ShowStockUpdates = false;
        }
    }

    [RelayCommand]
    private async Task RefreshDashboardAsync()
    {
        if (!IsConnected) return;

        try
        {
            await _realtimeService.RequestDashboardUpdateAsync();
            await ShowSuccessAsync("Dashboard refresh requested!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error refreshing dashboard: {ex.Message}");
        }
    }

    private void SubscribeToRealtimeEvents()
    {
        // Connection state changes
        _realtimeService.ConnectionStateChanged += OnConnectionStateChanged;

        // Stock updates
        _realtimeService.StockUpdated += OnStockUpdated;

        // Notifications
        _realtimeService.NotificationReceived += OnNotificationReceived;

        // User activities
        _realtimeService.UserActivityUpdated += OnUserActivityUpdated;

        // Dashboard updates
        _realtimeService.DashboardUpdated += OnDashboardUpdated;

        // Location updates
        _realtimeService.LocationUpdated += OnLocationUpdated;

        // User status changes
        _realtimeService.UserStatusChanged += OnUserStatusChanged;
    }

    private void OnConnectionStateChanged(object? sender, ConnectionEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsConnected = e.IsConnected;
            ConnectionStatus = e.IsConnected ? "Connected" :
                !string.IsNullOrEmpty(e.ErrorMessage) ? $"Error: {e.ErrorMessage}" : "Disconnected";
        });
    }

    private void OnStockUpdated(object? sender, StockUpdateEventArgs e)
    {
        if (!EnableStockUpdates) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Add to recent stock updates
            RecentStockUpdates.Insert(0, e);

            // Keep only last 50 updates
            while (RecentStockUpdates.Count > 50)
            {
                RecentStockUpdates.RemoveAt(RecentStockUpdates.Count - 1);
            }

            // Show notification for stock updates
            if (EnableNotifications)
            {
                var notification = new NotificationEventArgs
                {
                    Title = "Stock Update",
                    Message = $"{e.ProductName} ({e.ProductSKU}): {e.Action} - New stock: {e.NewStock}",
                    Type = NotificationType.StockAlert,
                    FromUserId = e.UserId,
                    FromUserName = e.UserName,
                    Data = new Dictionary<string, object>
                    {
                        ["productId"] = e.ProductId,
                        ["locationId"] = e.LocationId,
                        ["action"] = e.Action
                    },
                    ActionUrl = "product-detail",
                    ActionText = "View Product"
                };

                OnNotificationReceived(this, notification);
            }
        });
    }

    private void OnNotificationReceived(object? sender, NotificationEventArgs e)
    {
        if (!EnableNotifications) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Add to notifications
            Notifications.Insert(0, e);

            // Keep only last 100 notifications
            while (Notifications.Count > 100)
            {
                Notifications.RemoveAt(Notifications.Count - 1);
            }

            // Update unread count
            UpdateUnreadNotificationCount();

            // Show toast notification for important types
            if (e.Type == NotificationType.Error || e.Type == NotificationType.StockAlert)
            {
                _ = ShowErrorAsync(e.Message);
            }
            else if (e.Type == NotificationType.Warning)
            {
                _ = ShowWarningAsync(e.Message);
            }
        });
    }

    private void OnUserActivityUpdated(object? sender, UserActivityEventArgs e)
    {
        if (!EnableUserActivities) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Add to user activities
            UserActivities.Insert(0, e);

            // Keep only last 100 activities
            while (UserActivities.Count > 100)
            {
                UserActivities.RemoveAt(UserActivities.Count - 1);
            }
        });
    }

    private void OnDashboardUpdated(object? sender, DashboardUpdateEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Update dashboard metrics
            DashboardMetrics = e.Metrics;
            LastDashboardUpdate = e.LastUpdated;

            // Update stock alerts
            StockAlerts.Clear();
            foreach (var alert in e.StockAlerts)
            {
                StockAlerts.Add(alert);
            }

            // Add recent activities to user activities
            foreach (var activity in e.RecentActivities)
            {
                var userActivity = new UserActivityEventArgs
                {
                    UserId = activity.UserId,
                    UserName = activity.UserName,
                    Activity = activity.Action,
                    Details = activity.Details,
                    Timestamp = activity.Timestamp
                };

                UserActivities.Insert(0, userActivity);
            }

            // Trim user activities
            while (UserActivities.Count > 100)
            {
                UserActivities.RemoveAt(UserActivities.Count - 1);
            }
        });
    }

    private void OnLocationUpdated(object? sender, LocationUpdateEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Update user activity for location changes
            var userActivity = new UserActivityEventArgs
            {
                UserId = e.UserId,
                UserName = e.UserName,
                Activity = "Location Update",
                Details = $"Moved to {e.LocationName}",
                LocationId = e.LocationId,
                LocationName = e.LocationName,
                Timestamp = e.Timestamp
            };

            OnUserActivityUpdated(this, userActivity);
        });
    }

    private void OnUserStatusChanged(object? sender, UserStatusEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Update online users list
            var existingUser = OnlineUsers.FirstOrDefault(u => u.UserId == e.UserId);

            if (existingUser != null)
            {
                OnlineUsers.Remove(existingUser);
            }

            if (e.Status != UserStatus.Offline)
            {
                OnlineUsers.Add(e);
            }

            OnlineUserCount = OnlineUsers.Count;

            // Update current user status if it's the current user
            if (e.UserId == CurrentUserId)
            {
                CurrentUserStatus = e.Status;
                CurrentLocationId = e.LocationId;
                CurrentLocationName = e.LocationName;
            }
        });
    }

    private void UpdateUnreadNotificationCount()
    {
        UnreadNotificationCount = Notifications.Count(n => !n.IsRead);
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

            // Unsubscribe from events
            _realtimeService.ConnectionStateChanged -= OnConnectionStateChanged;
            _realtimeService.StockUpdated -= OnStockUpdated;
            _realtimeService.NotificationReceived -= OnNotificationReceived;
            _realtimeService.UserActivityUpdated -= OnUserActivityUpdated;
            _realtimeService.DashboardUpdated -= OnDashboardUpdated;
            _realtimeService.LocationUpdated -= OnLocationUpdated;
            _realtimeService.UserStatusChanged -= OnUserStatusChanged;

            // Disconnect if connected
            if (IsConnected)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _realtimeService.DisconnectAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error disconnecting during dispose");
                    }
                });
            }
        }
    }

    ~RealtimeViewModel()
    {
        Dispose(false);
    }

    // Property change handlers
    partial void OnSelectedGroupChanged(string value)
    {
        if (IsConnected && !string.IsNullOrEmpty(value))
        {
            _ = JoinGroupAsync(value);
        }
    }

    partial void OnCurrentLocationIdChanged(string value)
    {
        if (IsConnected && !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(CurrentLocationName))
        {
            _ = _realtimeService.SendLocationUpdateAsync(value, CurrentLocationName);
        }
    }
}