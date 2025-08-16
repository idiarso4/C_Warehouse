namespace WarehouseApp.MAUI.Services;

public interface INavigationService
{
    // Basic navigation
    Task NavigateToAsync(string route);
    Task NavigateToAsync(string route, IDictionary<string, object> parameters);
    Task GoToAsync(string route);
    Task GoToAsync(string route, IDictionary<string, object> parameters);
    Task GoBackAsync();
    Task GoToRootAsync();

    // Modal navigation
    Task PushModalAsync(string route);
    Task PushModalAsync(string route, IDictionary<string, object> parameters);
    Task PopModalAsync();

    // Tab navigation
    Task SwitchToTabAsync(string tabRoute);

    // Deep linking
    Task NavigateToAsync(Uri uri);

    // Navigation with result
    Task<T?> NavigateToAsync<T>(string route);
    Task<T?> NavigateToAsync<T>(string route, IDictionary<string, object> parameters);

    // Navigation stack management
    Task ClearNavigationStackAsync();
    Task RemoveFromStackAsync(string route);
    bool CanGoBack { get; }

    // Current page info
    string CurrentRoute { get; }
    IDictionary<string, object>? CurrentParameters { get; }

    // Events
    event EventHandler<NavigationEventArgs>? Navigating;
    event EventHandler<NavigationEventArgs>? Navigated;
}

public class NavigationEventArgs : EventArgs
{
    public string Route { get; set; } = string.Empty;
    public IDictionary<string, object>? Parameters { get; set; }
    public bool Cancel { get; set; }
}