using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using WarehouseApp.Core.Models;
using WarehouseApp.MAUI.Services;

namespace WarehouseApp.MAUI.ViewModels;

public partial class AdvancedSearchViewModel : BaseViewModel
{
    private readonly ISearchService _searchService;
    private readonly INavigationService _navigationService;
    private readonly ILogger<AdvancedSearchViewModel> _logger;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private SearchEntityType _selectedEntityType = SearchEntityType.Product;

    [ObservableProperty]
    private ObservableCollection<SearchEntityType> _entityTypes = new()
    {
        SearchEntityType.Product,
        SearchEntityType.Category,
        SearchEntityType.Location,
        SearchEntityType.StockMovement,
        SearchEntityType.All
    };

    [ObservableProperty]
    private ObservableCollection<SearchSuggestion> _searchSuggestions = new();

    [ObservableProperty]
    private ObservableCollection<SearchHistoryItem> _searchHistory = new();

    [ObservableProperty]
    private bool _showSuggestions = false;

    [ObservableProperty]
    private bool _showHistory = false;

    [ObservableProperty]
    private bool _showFilters = false;

    [ObservableProperty]
    private SearchFilters _availableFilters = new();

    [ObservableProperty]
    private Dictionary<string, object> _activeFilters = new();

    [ObservableProperty]
    private string _sortBy = "name";

    [ObservableProperty]
    private SortDirection _sortDirection = SortDirection.Ascending;

    [ObservableProperty]
    private ObservableCollection<string> _sortOptions = new();

    [ObservableProperty]
    private bool _includeInactive = false;

    [ObservableProperty]
    private int _pageSize = 20;

    [ObservableProperty]
    private ObservableCollection<int> _pageSizeOptions = new() { 10, 20, 50, 100 };

    // Search Results
    [ObservableProperty]
    private ObservableCollection<object> _searchResults = new();

    [ObservableProperty]
    private int _totalResults = 0;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 0;

    [ObservableProperty]
    private bool _hasNextPage = false;

    [ObservableProperty]
    private bool _hasPreviousPage = false;

    [ObservableProperty]
    private TimeSpan _searchTime;

    [ObservableProperty]
    private Dictionary<string, int> _searchFacets = new();

    [ObservableProperty]
    private bool _isSearching = false;

    [ObservableProperty]
    private string _searchStatus = string.Empty;

    public AdvancedSearchViewModel(
        ISearchService searchService,
        INavigationService navigationService,
        ILogger<AdvancedSearchViewModel> logger)
    {
        _searchService = searchService;
        _navigationService = navigationService;
        _logger = logger;

        Title = "Advanced Search";
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await LoadSearchHistory();
        await LoadAvailableFilters();
        UpdateSortOptions();
    }

    [RelayCommand]
    private async Task PerformSearchAsync()
    {
        if (IsSearching || string.IsNullOrWhiteSpace(SearchQuery))
            return;

        IsSearching = true;
        SearchStatus = "Searching...";

        try
        {
            var criteria = new SearchCriteria
            {
                Query = SearchQuery.Trim(),
                EntityType = SelectedEntityType,
                Filters = new Dictionary<string, object>(ActiveFilters),
                SortBy = SortBy,
                SortDirection = SortDirection,
                PageNumber = CurrentPage,
                PageSize = PageSize,
                IncludeInactive = IncludeInactive
            };

            switch (SelectedEntityType)
            {
                case SearchEntityType.Product:
                    await SearchProductsAsync(criteria);
                    break;
                case SearchEntityType.Category:
                    await SearchCategoriesAsync(criteria);
                    break;
                case SearchEntityType.Location:
                    await SearchLocationsAsync(criteria);
                    break;
                case SearchEntityType.StockMovement:
                    await SearchStockMovementsAsync(criteria);
                    break;
                case SearchEntityType.All:
                    await SearchAllAsync(criteria);
                    break;
            }

            // Hide suggestions and history after search
            ShowSuggestions = false;
            ShowHistory = false;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Search failed: {ex.Message}");
            SearchStatus = "Search failed";
        }
        finally
        {
            IsSearching = false;
        }
    }

    [RelayCommand]
    private async Task ClearSearchResultsAsync()
    {
        SearchQuery = string.Empty;
        SearchResults.Clear();
        SearchFacets.Clear();
        ActiveFilters.Clear();
        TotalResults = 0;
        CurrentPage = 1;
        TotalPages = 0;
        HasNextPage = false;
        HasPreviousPage = false;
        SearchStatus = string.Empty;
        ShowSuggestions = false;
        ShowHistory = false;
    }

    [RelayCommand]
    private async Task LoadSuggestionsAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery) || SearchQuery.Length < 2)
        {
            ShowSuggestions = false;
            return;
        }

        try
        {
            var suggestions = await _searchService.GetSearchSuggestionsAsync(SearchQuery, SelectedEntityType);

            SearchSuggestions.Clear();
            foreach (var suggestion in suggestions)
            {
                SearchSuggestions.Add(suggestion);
            }

            ShowSuggestions = SearchSuggestions.Any();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error loading search suggestions");
        }
    }

    [RelayCommand]
    private async Task SelectSuggestionAsync(SearchSuggestion suggestion)
    {
        if (suggestion == null) return;

        SearchQuery = suggestion.Text;
        SelectedEntityType = suggestion.EntityType;
        ShowSuggestions = false;

        await PerformSearchAsync();
    }

    [RelayCommand]
    private async Task SelectHistoryItemAsync(SearchHistoryItem historyItem)
    {
        if (historyItem == null) return;

        SearchQuery = historyItem.Query;
        SelectedEntityType = historyItem.EntityType;
        ShowHistory = false;

        await PerformSearchAsync();
    }

    [RelayCommand]
    private void ToggleHistory()
    {
        ShowHistory = !ShowHistory;
        if (ShowHistory)
        {
            ShowSuggestions = false;
            _ = LoadSearchHistory();
        }
    }

    [RelayCommand]
    private void ToggleFilters()
    {
        ShowFilters = !ShowFilters;
    }

    public async Task ApplyFilterAsync(string filterName, object filterValue)
    {
        if (string.IsNullOrEmpty(filterName)) return;

        if (filterValue == null)
        {
            ActiveFilters.Remove(filterName);
        }
        else
        {
            ActiveFilters[filterName] = filterValue;
        }

        // Reset to first page when filters change
        CurrentPage = 1;

        // Re-search with new filters
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            await PerformSearchAsync();
        }
    }

    [RelayCommand]
    private async Task ClearFiltersAsync()
    {
        ActiveFilters.Clear();
        CurrentPage = 1;

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            await PerformSearchAsync();
        }
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (!HasNextPage) return;

        CurrentPage++;
        await PerformSearchAsync();
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (!HasPreviousPage) return;

        CurrentPage--;
        await PerformSearchAsync();
    }

    [RelayCommand]
    private async Task GoToPageAsync(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > TotalPages) return;

        CurrentPage = pageNumber;
        await PerformSearchAsync();
    }

    [RelayCommand]
    private async Task ChangeSortAsync(string sortBy)
    {
        if (SortBy == sortBy)
        {
            // Toggle sort direction
            SortDirection = SortDirection == SortDirection.Ascending
                ? SortDirection.Descending
                : SortDirection.Ascending;
        }
        else
        {
            SortBy = sortBy;
            SortDirection = SortDirection.Ascending;
        }

        CurrentPage = 1;

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            await PerformSearchAsync();
        }
    }

    [RelayCommand]
    private async Task ViewItemDetailsAsync(object item)
    {
        if (item == null) return;

        try
        {
            switch (item)
            {
                case Product product:
                    await _navigationService.GoToAsync("product-detail", new Dictionary<string, object>
                    {
                        ["productId"] = product.Id
                    });
                    break;
                case Category category:
                    await _navigationService.GoToAsync("category-detail", new Dictionary<string, object>
                    {
                        ["categoryId"] = category.Id
                    });
                    break;
                case WarehouseApp.Core.Models.Location location:
                    await _navigationService.GoToAsync("location-detail", new Dictionary<string, object>
                    {
                        ["locationId"] = location.Id
                    });
                    break;
                case StockMovement stockMovement:
                    await _navigationService.GoToAsync("stock-movement-detail", new Dictionary<string, object>
                    {
                        ["movementId"] = stockMovement.Id
                    });
                    break;
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error navigating to item details: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ClearSearchHistoryAsync()
    {
        var confirmed = await ShowConfirmAsync(
            "Clear Search History",
            "Are you sure you want to clear all search history?");

        if (!confirmed) return;

        try
        {
            await _searchService.ClearSearchHistoryAsync();
            SearchHistory.Clear();
            await ShowSuccessAsync("Search history cleared successfully!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error clearing search history: {ex.Message}");
        }
    }

    private async Task SearchProductsAsync(SearchCriteria criteria)
    {
        var result = await _searchService.SearchProductsAsync(criteria);
        UpdateSearchResults(result, result.Items.Cast<object>());
    }

    private async Task SearchCategoriesAsync(SearchCriteria criteria)
    {
        var result = await _searchService.SearchCategoriesAsync(criteria);
        UpdateSearchResults(result, result.Items.Cast<object>());
    }

    private async Task SearchLocationsAsync(SearchCriteria criteria)
    {
        var result = await _searchService.SearchLocationsAsync(criteria);
        UpdateSearchResults(result, result.Items.Cast<object>());
    }

    private async Task SearchStockMovementsAsync(SearchCriteria criteria)
    {
        var result = await _searchService.SearchStockMovementsAsync(criteria);
        UpdateSearchResults(result, result.Items.Cast<object>());
    }

    private async Task SearchAllAsync(SearchCriteria criteria)
    {
        // Search across all entity types and combine results
        var allResults = new List<object>();
        var totalCount = 0;
        var searchTime = TimeSpan.Zero;

        try
        {
            var productCriteria = new SearchCriteria
            {
                Query = criteria.Query,
                EntityType = SearchEntityType.Product,
                Filters = criteria.Filters,
                SortBy = criteria.SortBy,
                SortDirection = criteria.SortDirection,
                PageNumber = 1,
                PageSize = criteria.PageSize / 4, // Divide results among entity types
                IncludeInactive = criteria.IncludeInactive
            };

            var productResult = await _searchService.SearchProductsAsync(productCriteria);
            allResults.AddRange(productResult.Items.Cast<object>());
            totalCount += productResult.TotalCount;
            searchTime = searchTime.Add(productResult.SearchTime);

            var categoryCriteria = new SearchCriteria
            {
                Query = criteria.Query,
                EntityType = SearchEntityType.Category,
                Filters = criteria.Filters,
                SortBy = criteria.SortBy,
                SortDirection = criteria.SortDirection,
                PageNumber = 1,
                PageSize = criteria.PageSize / 4,
                IncludeInactive = criteria.IncludeInactive
            };

            var categoryResult = await _searchService.SearchCategoriesAsync(categoryCriteria);
            allResults.AddRange(categoryResult.Items.Cast<object>());
            totalCount += categoryResult.TotalCount;
            searchTime = searchTime.Add(categoryResult.SearchTime);

            var locationCriteria = new SearchCriteria
            {
                Query = criteria.Query,
                EntityType = SearchEntityType.Location,
                Filters = criteria.Filters,
                SortBy = criteria.SortBy,
                SortDirection = criteria.SortDirection,
                PageNumber = 1,
                PageSize = criteria.PageSize / 4,
                IncludeInactive = criteria.IncludeInactive
            };

            var locationResult = await _searchService.SearchLocationsAsync(locationCriteria);
            allResults.AddRange(locationResult.Items.Cast<object>());
            totalCount += locationResult.TotalCount;
            searchTime = searchTime.Add(locationResult.SearchTime);

            // Create combined result
            var combinedResult = new SearchResult<object>
            {
                Items = allResults,
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                SearchTime = searchTime
            };

            UpdateSearchResults(combinedResult, allResults);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in combined search: {ex.Message}", ex);
        }
    }

    private void UpdateSearchResults<T>(SearchResult<T> result, IEnumerable<object> items)
    {
        SearchResults.Clear();
        foreach (var item in items)
        {
            SearchResults.Add(item);
        }

        TotalResults = result.TotalCount;
        CurrentPage = result.PageNumber;
        TotalPages = result.TotalPages;
        HasNextPage = result.HasNextPage;
        HasPreviousPage = result.HasPreviousPage;
        SearchTime = result.SearchTime;
        SearchFacets = result.Facets;

        SearchStatus = $"Found {TotalResults:N0} results in {SearchTime.TotalMilliseconds:F0}ms";
    }

    private async Task LoadSearchHistory()
    {
        try
        {
            var history = await _searchService.GetSearchHistoryAsync(SelectedEntityType);

            SearchHistory.Clear();
            foreach (var item in history)
            {
                SearchHistory.Add(item);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error loading search history");
        }
    }

    private async Task LoadAvailableFilters()
    {
        try
        {
            AvailableFilters = await _searchService.GetAvailableFiltersAsync(SelectedEntityType);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error loading available filters");
        }
    }

    private void UpdateSortOptions()
    {
        SortOptions.Clear();

        var options = SelectedEntityType switch
        {
            SearchEntityType.Product => new[] { "name", "sku", "price", "stock", "created", "updated" },
            SearchEntityType.Category => new[] { "name", "created" },
            SearchEntityType.Location => new[] { "name", "code", "created" },
            SearchEntityType.StockMovement => new[] { "date", "quantity", "type" },
            _ => new[] { "name", "created" }
        };

        foreach (var option in options)
        {
            SortOptions.Add(option);
        }
    }

    partial void OnSelectedEntityTypeChanged(SearchEntityType value)
    {
        UpdateSortOptions();
        _ = LoadAvailableFilters();
        _ = LoadSearchHistory();

        // Clear current results when entity type changes
        SearchResults.Clear();
        SearchFacets.Clear();
        ActiveFilters.Clear();
        TotalResults = 0;
        CurrentPage = 1;
    }

    partial void OnSearchQueryChanged(string value)
    {
        // Debounce suggestions loading
        _ = Task.Delay(300).ContinueWith(async _ =>
        {
            if (SearchQuery == value) // Check if query hasn't changed
            {
                await LoadSuggestionsAsync();
            }
        });
    }
}