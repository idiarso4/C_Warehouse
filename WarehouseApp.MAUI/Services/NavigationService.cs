namespace WarehouseApp.MAUI.Services;

public class NavigationService : INavigationService
{
    private readonly Dictionary<string, object> _navigationResults = new();
    private string _currentRoute = string.Empty;
    private IDictionary<string, object>? _currentParameters;

    public event EventHandler<NavigationEventArgs>? Navigating;
    public event EventHandler<NavigationEventArgs>? Navigated;

    public string CurrentRoute => _currentRoute;
    public IDictionary<string, object>? CurrentParameters => _currentParameters;
    public bool CanGoBack => Shell.Current?.Navigation?.NavigationStack?.Count > 1;

    // Basic navigation
    public async Task NavigateToAsync(string route)
    {
        await NavigateToAsync(route, new Dictionary<string, object>());
    }

    public async Task NavigateToAsync(string route, IDictionary<string, object> parameters)
    {
        try
        {
            var args = new NavigationEventArgs { Route = route, Parameters = parameters };
            Navigating?.Invoke(this, args);

            if (args.Cancel) return;

            await Shell.Current.GoToAsync(route, parameters);

            _currentRoute = route;
            _currentParameters = parameters;

            Navigated?.Invoke(this, args);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            throw;
        }
    }

    public async Task GoToAsync(string route)
    {
        await NavigateToAsync(route);
    }

    public async Task GoToAsync(string route, IDictionary<string, object> parameters)
    {
        await NavigateToAsync(route, parameters);
    }

    public async Task GoBackAsync()
    {
        try
        {
            if (CanGoBack)
            {
                await Shell.Current.GoToAsync("..");
                UpdateCurrentRoute();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Go back error: {ex.Message}");
            throw;
        }
    }

    public async Task GoToRootAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("//");
            _currentRoute = string.Empty;
            _currentParameters = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Go to root error: {ex.Message}");
            throw;
        }
    }

    // Modal navigation
    public async Task PushModalAsync(string route)
    {
        await PushModalAsync(route, new Dictionary<string, object>());
    }

    public async Task PushModalAsync(string route, IDictionary<string, object> parameters)
    {
        try
        {
            var args = new NavigationEventArgs { Route = route, Parameters = parameters };
            Navigating?.Invoke(this, args);

            if (args.Cancel) return;

            // For modal navigation, we'll use Shell navigation with modal parameter
            var modalRoute = $"{route}?presentationMode=Modal";
            await Shell.Current.GoToAsync(modalRoute, parameters);

            _currentRoute = route;
            _currentParameters = parameters;

            Navigated?.Invoke(this, args);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Push modal error: {ex.Message}");
            throw;
        }
    }

    public async Task PopModalAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("..");
            UpdateCurrentRoute();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Pop modal error: {ex.Message}");
            throw;
        }
    }

    // Tab navigation
    public async Task SwitchToTabAsync(string tabRoute)
    {
        try
        {
            await Shell.Current.GoToAsync($"//{tabRoute}");
            _currentRoute = tabRoute;
            _currentParameters = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Switch tab error: {ex.Message}");
            throw;
        }
    }

    // Deep linking
    public async Task NavigateToAsync(Uri uri)
    {
        try
        {
            await Shell.Current.GoToAsync(uri.ToString());
            _currentRoute = uri.ToString();
            _currentParameters = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Deep link navigation error: {ex.Message}");
            throw;
        }
    }

    // Navigation with result
    public async Task<T?> NavigateToAsync<T>(string route)
    {
        return await NavigateToAsync<T>(route, new Dictionary<string, object>());
    }

    public async Task<T?> NavigateToAsync<T>(string route, IDictionary<string, object> parameters)
    {
        try
        {
            var resultKey = Guid.NewGuid().ToString();
            parameters["ResultKey"] = resultKey;

            await NavigateToAsync(route, parameters);

            // Wait for result (this is a simplified implementation)
            // In a real app, you might want to use TaskCompletionSource
            await Task.Delay(100); // Small delay to allow navigation to complete

            if (_navigationResults.TryGetValue(resultKey, out var result))
            {
                _navigationResults.Remove(resultKey);
                return (T?)result;
            }

            return default(T);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Navigation with result error: {ex.Message}");
            throw;
        }
    }

    // Navigation stack management
    public async Task ClearNavigationStackAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("//");
            _currentRoute = string.Empty;
            _currentParameters = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Clear navigation stack error: {ex.Message}");
            throw;
        }
    }

    public async Task RemoveFromStackAsync(string route)
    {
        try
        {
            // This is a simplified implementation
            // In a real app, you might need more sophisticated stack management
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Remove from stack error: {ex.Message}");
            throw;
        }
    }

    // Helper methods
    private void UpdateCurrentRoute()
    {
        try
        {
            // Try to get current route from Shell
            var currentPage = Shell.Current?.CurrentPage;
            if (currentPage != null)
            {
                _currentRoute = currentPage.GetType().Name;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Update current route error: {ex.Message}");
        }
    }

    // Method to set navigation result
    public void SetNavigationResult<T>(string resultKey, T result)
    {
        if (!string.IsNullOrEmpty(resultKey) && result != null)
        {
            _navigationResults[resultKey] = result;
        }
    }

    // Method to get navigation result
    public T? GetNavigationResult<T>(string resultKey)
    {
        if (_navigationResults.TryGetValue(resultKey, out var result))
        {
            _navigationResults.Remove(resultKey);
            return (T?)result;
        }
        return default(T);
    }
}