using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WarehouseApp.Core.Interfaces;
using WarehouseApp.MAUI.Services;

namespace WarehouseApp.MAUI.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IProductService _productService;
    private readonly IStockService _stockService;

    [ObservableProperty]
    private int _totalProducts;

    [ObservableProperty]
    private int _lowStockItems;

    [ObservableProperty]
    private int _totalCategories;

    [ObservableProperty]
    private int _totalLocations;

    [ObservableProperty]
    private string _lastSyncTime = "Never";

    [ObservableProperty]
    private bool _isOnline = true;

    [ObservableProperty]
    private ObservableCollection<DashboardItem> _quickActions = new();

    [ObservableProperty]
    private ObservableCollection<RecentActivity> _recentActivities = new();

    public MainViewModel(
        INavigationService navigationService,
        IProductService productService,
        IStockService stockService)
    {
        _navigationService = navigationService;
        _productService = productService;
        _stockService = stockService;

        Title = "Warehouse Dashboard";
        InitializeQuickActions();
    }

    public override async Task InitializeAsync()
    {
        await SetBusyAsync(async () =>
        {
            await LoadDashboardDataAsync();
            await LoadRecentActivitiesAsync();
        }, "Loading dashboard...");
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            // Load dashboard statistics
            // These would be real API calls in production
            TotalProducts = 150; // await _productService.GetTotalCountAsync();
            LowStockItems = 12; // await _productService.GetLowStockCountAsync();
            TotalCategories = 8; // await _categoryService.GetTotalCountAsync();
            TotalLocations = 25; // await _locationService.GetTotalCountAsync();

            LastSyncTime = DateTime.Now.ToString("HH:mm");
            IsOnline = true; // Check actual connectivity
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task LoadRecentActivitiesAsync()
    {
        try
        {
            RecentActivities.Clear();

            // Sample data - replace with real data
            var activities = new List<RecentActivity>
            {
                new() { Description = "Added 50 units of Product A", Time = DateTime.Now.AddMinutes(-5), Type = ActivityType.StockIn },
                new() { Description = "Moved Product B to Location A1", Time = DateTime.Now.AddMinutes(-15), Type = ActivityType.Transfer },
                new() { Description = "Created new category: Electronics", Time = DateTime.Now.AddHours(-1), Type = ActivityType.Create },
                new() { Description = "Updated Product C details", Time = DateTime.Now.AddHours(-2), Type = ActivityType.Update }
            };

            foreach (var activity in activities)
            {
                RecentActivities.Add(activity);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void InitializeQuickActions()
    {
        QuickActions.Clear();
        QuickActions.Add(new DashboardItem { Title = "Add Product", Icon = "plus_circle", Command = AddProductCommand });
        QuickActions.Add(new DashboardItem { Title = "Stock In", Icon = "arrow_down", Command = StockInCommand });
        QuickActions.Add(new DashboardItem { Title = "Stock Out", Icon = "arrow_up", Command = StockOutCommand });
        QuickActions.Add(new DashboardItem { Title = "Transfer", Icon = "swap", Command = TransferCommand });
        QuickActions.Add(new DashboardItem { Title = "Scan Barcode", Icon = "barcode", Command = ScanBarcodeCommand });
        QuickActions.Add(new DashboardItem { Title = "Reports", Icon = "chart", Command = ViewReportsCommand });
    }

    [RelayCommand]
    private async Task NavigateToProductsAsync()
    {
        await _navigationService.NavigateToAsync("//products");
    }

    [RelayCommand]
    private async Task NavigateToCategoriesAsync()
    {
        await _navigationService.NavigateToAsync("//categories");
    }

    [RelayCommand]
    private async Task NavigateToLocationsAsync()
    {
        await _navigationService.NavigateToAsync("//locations");
    }

    [RelayCommand]
    private async Task NavigateToReportsAsync()
    {
        await _navigationService.NavigateToAsync("//reports");
    }

    [RelayCommand]
    private async Task AddProductAsync()
    {
        await _navigationService.NavigateToAsync("//products/add");
    }

    [RelayCommand]
    private async Task StockInAsync()
    {
        await _navigationService.NavigateToAsync("//stock/in");
    }

    [RelayCommand]
    private async Task StockOutAsync()
    {
        await _navigationService.NavigateToAsync("//stock/out");
    }

    [RelayCommand]
    private async Task TransferAsync()
    {
        await _navigationService.NavigateToAsync("//stock/transfer");
    }

    [RelayCommand]
    private async Task ScanBarcodeAsync()
    {
        await _navigationService.NavigateToAsync("//scanner");
    }

    [RelayCommand]
    private async Task ViewReportsAsync()
    {
        await _navigationService.NavigateToAsync("//reports");
    }

    [RelayCommand]
    private async Task SyncDataAsync()
    {
        await SetBusyAsync(async () =>
        {
            // Simulate sync operation
            await Task.Delay(2000);
            LastSyncTime = DateTime.Now.ToString("HH:mm");
            await LoadDashboardDataAsync();
        }, "Syncing data...");
    }
}

// Helper classes
public class DashboardItem
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public IRelayCommand? Command { get; set; }
}

public class RecentActivity
{
    public string Description { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public ActivityType Type { get; set; }
    public string TimeAgo => GetTimeAgo(Time);

    private static string GetTimeAgo(DateTime time)
    {
        var span = DateTime.Now - time;
        return span.TotalMinutes < 1 ? "Just now" :
               span.TotalMinutes < 60 ? $"{(int)span.TotalMinutes}m ago" :
               span.TotalHours < 24 ? $"{(int)span.TotalHours}h ago" :
               $"{(int)span.TotalDays}d ago";
    }
}

public enum ActivityType
{
    StockIn,
    StockOut,
    Transfer,
    Create,
    Update,
    Delete
}