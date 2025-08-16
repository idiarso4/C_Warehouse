using WarehouseApp.Core.Models;

namespace WarehouseApp.MAUI.Services;

public interface ISearchService
{
    /// <summary>
    /// Search products with advanced filters
    /// </summary>
    /// <param name="criteria">Search criteria</param>
    /// <returns>Search results with metadata</returns>
    Task<SearchResult<Product>> SearchProductsAsync(SearchCriteria criteria);

    /// <summary>
    /// Search categories with filters
    /// </summary>
    /// <param name="criteria">Search criteria</param>
    /// <returns>Search results with metadata</returns>
    Task<SearchResult<Category>> SearchCategoriesAsync(SearchCriteria criteria);

    /// <summary>
    /// Search locations with filters
    /// </summary>
    /// <param name="criteria">Search criteria</param>
    /// <returns>Search results with metadata</returns>
    Task<SearchResult<WarehouseApp.Core.Models.Location>> SearchLocationsAsync(SearchCriteria criteria);

    /// <summary>
    /// Search stock movements with filters
    /// </summary>
    /// <param name="criteria">Search criteria</param>
    /// <returns>Search results with metadata</returns>
    Task<SearchResult<StockMovement>> SearchStockMovementsAsync(SearchCriteria criteria);

    /// <summary>
    /// Get search suggestions based on query
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="entityType">Type of entity to search</param>
    /// <returns>List of suggestions</returns>
    Task<IEnumerable<SearchSuggestion>> GetSearchSuggestionsAsync(string query, SearchEntityType entityType);

    /// <summary>
    /// Get available filter options for entity type
    /// </summary>
    /// <param name="entityType">Type of entity</param>
    /// <returns>Available filters</returns>
    Task<SearchFilters> GetAvailableFiltersAsync(SearchEntityType entityType);

    /// <summary>
    /// Save search query to history
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="entityType">Entity type</param>
    Task SaveSearchHistoryAsync(string query, SearchEntityType entityType);

    /// <summary>
    /// Get search history
    /// </summary>
    /// <param name="entityType">Entity type filter</param>
    /// <returns>Recent search queries</returns>
    Task<IEnumerable<SearchHistoryItem>> GetSearchHistoryAsync(SearchEntityType? entityType = null);

    /// <summary>
    /// Clear search history
    /// </summary>
    Task ClearSearchHistoryAsync();
}

public class SearchCriteria
{
    public string Query { get; set; } = string.Empty;
    public SearchEntityType EntityType { get; set; }
    public Dictionary<string, object> Filters { get; set; } = new();
    public string SortBy { get; set; } = string.Empty;
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool IncludeInactive { get; set; } = false;
}

public class SearchResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
    public Dictionary<string, int> Facets { get; set; } = new();
    public TimeSpan SearchTime { get; set; }
}

public class SearchSuggestion
{
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public SearchEntityType EntityType { get; set; }
}

public class SearchFilters
{
    public Dictionary<string, FilterOption> AvailableFilters { get; set; } = new();
}

public class FilterOption
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public FilterType Type { get; set; }
    public IEnumerable<FilterValue> Values { get; set; } = new List<FilterValue>();
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }
}

public class FilterValue
{
    public string Value { get; set; } = string.Empty;
    public string DisplayText { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class SearchHistoryItem
{
    public string Query { get; set; } = string.Empty;
    public SearchEntityType EntityType { get; set; }
    public DateTime SearchedAt { get; set; }
    public int ResultCount { get; set; }
}

public enum SearchEntityType
{
    Product,
    Category,
    Location,
    StockMovement,
    User,
    All
}

public enum SortDirection
{
    Ascending,
    Descending
}

public enum FilterType
{
    Text,
    Number,
    Date,
    Boolean,
    Select,
    MultiSelect,
    Range
}