using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WarehouseApp.MAUI.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _emptyMessage = "No data available";

    [ObservableProperty]
    private bool _isConnected = true;

    [ObservableProperty]
    private string _searchText = string.Empty;

    // Loading state management
    public virtual async Task SetBusyAsync(Func<Task> operation, string? loadingMessage = null)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            if (!string.IsNullOrEmpty(loadingMessage))
            {
                Subtitle = loadingMessage;
            }

            await operation();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsBusy = false;
            IsLoading = false;
            Subtitle = string.Empty;
        }
    }

    // Error handling
    public virtual async Task HandleErrorAsync(Exception exception)
    {
        HasError = true;
        ErrorMessage = GetUserFriendlyErrorMessage(exception);

        // Log error (implement logging service)
        System.Diagnostics.Debug.WriteLine($"Error in {GetType().Name}: {exception}");

        // Show error to user
        await ShowErrorAsync(ErrorMessage);
    }

    protected virtual string GetUserFriendlyErrorMessage(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => "You don't have permission to perform this action.",
            TimeoutException => "The operation timed out. Please check your connection and try again.",
            HttpRequestException => "Network error. Please check your internet connection.",
            ArgumentException => "Invalid input provided.",
            InvalidOperationException => "This operation cannot be performed at this time.",
            _ => "An unexpected error occurred. Please try again."
        };
    }

    // Virtual methods for derived classes to override
    public virtual async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public virtual async Task RefreshAsync()
    {
        if (IsRefreshing) return;

        try
        {
            IsRefreshing = true;
            HasError = false;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    protected virtual async Task LoadDataAsync()
    {
        await Task.CompletedTask;
    }

    protected virtual async Task ShowErrorAsync(string message)
    {
        // This will be implemented with proper dialog service
        if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
        {
            await Application.Current.Windows.FirstOrDefault()!.Page.DisplayAlert("Error", message, "OK");
        }
    }

    protected virtual async Task ShowSuccessAsync(string message)
    {
        // This will be implemented with proper dialog service
        if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
        {
            await Application.Current.Windows.FirstOrDefault()!.Page.DisplayAlert("Success", message, "OK");
        }
    }

    protected virtual async Task<bool> ShowConfirmAsync(string title, string message)
    {
        // This will be implemented with proper dialog service
        if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
        {
            return await Application.Current.Windows.FirstOrDefault()!.Page.DisplayAlert(title, message, "Yes", "No");
        }
        return false;
    }

    protected virtual async Task ShowInfoAsync(string message)
    {
        // This will be implemented with proper dialog service
        if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
        {
            await Application.Current.Windows.FirstOrDefault()!.Page.DisplayAlert("Info", message, "OK");
        }
    }

    protected virtual async Task ShowWarningAsync(string message)
    {
        // This will be implemented with proper dialog service
        if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
        {
            await Application.Current.Windows.FirstOrDefault()!.Page.DisplayAlert("Warning", message, "OK");
        }
    }

    // Search functionality
    [RelayCommand]
    private async Task SearchAsync()
    {
        await PerformSearchAsync(SearchText);
    }

    [RelayCommand]
    private async Task ClearSearchAsync()
    {
        SearchText = string.Empty;
        await PerformSearchAsync(string.Empty);
    }

    protected virtual async Task PerformSearchAsync(string searchTerm)
    {
        await Task.CompletedTask;
    }

    // Navigation helpers
    protected virtual async Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        try
        {
            if (parameters != null)
            {
                await Shell.Current.GoToAsync(route, parameters);
            }
            else
            {
                await Shell.Current.GoToAsync(route);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual async Task NavigateBackAsync()
    {
        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    // Cleanup
    public virtual void OnDisappearing()
    {
        // Override in derived classes for cleanup
    }

    public virtual void OnAppearing()
    {
        // Override in derived classes for initialization
    }
}