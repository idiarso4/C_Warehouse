using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseApp.Core.Models;
using WarehouseApp.Data.Interfaces;
using WarehouseApp.Data.Repositories;

namespace WarehouseApp.MAUI.Services;

public class SearchService : ISearchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SearchService> _logger;
    private readonly List<SearchHistoryItem> _searchHistory = new();

    public SearchService(IUnitOfWork unitOfWork, ILogger<SearchService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SearchResult<Product>> SearchProductsAsync(SearchCriteria criteria)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation($"Searching products with query: {criteria.Query}");

            var query = _unitOfWork.Products.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Location)
                .AsQueryable();

            // Apply text search
            if (!string.IsNullOrWhiteSpace(criteria.Query))
            {
                var searchTerm = criteria.Query.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.SKU.ToLower().Contains(searchTerm) ||
                    p.Barcode.ToLower().Contains(searchTerm) ||
                    (p.Category != null && p.Category.Name.ToLower().Contains(searchTerm)) ||
                    (p.Location != null && p.Location.Name.ToLower().Contains(searchTerm)));
            }

            // Apply filters
            query = ApplyProductFilters(query, criteria.Filters);

            // Apply active/inactive filter
            if (!criteria.IncludeInactive)
            {
                query = query.Where(p => p.IsActive);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplyProductSorting(query, criteria.SortBy, criteria.SortDirection);

            // Apply pagination
            var items = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            // Generate facets
            var facets = await GenerateProductFacets(criteria);

            stopwatch.Stop();

            // Save to search history
            await SaveSearchHistoryAsync(criteria.Query, SearchEntityType.Product);

            return new SearchResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                Facets = facets,
                SearchTime = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error searching products: {criteria.Query}");
            throw;
        }
    }

    public async Task<SearchResult<Category>> SearchCategoriesAsync(SearchCriteria criteria)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation($"Searching categories with query: {criteria.Query}");

            var query = _unitOfWork.Categories.GetQueryable()
                .Include(c => c.Parent)
                .Include(c => c.Children)
                .AsQueryable();

            // Apply text search
            if (!string.IsNullOrWhiteSpace(criteria.Query))
            {
                var searchTerm = criteria.Query.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Description.ToLower().Contains(searchTerm) ||
                    (c.Parent != null && c.Parent.Name.ToLower().Contains(searchTerm)));
            }

            // Apply filters
            query = ApplyCategoryFilters(query, criteria.Filters);

            // Apply active/inactive filter
            if (!criteria.IncludeInactive)
            {
                query = query.Where(c => c.IsActive);
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplyCategorySorting(query, criteria.SortBy, criteria.SortDirection);

            // Apply pagination
            var items = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            stopwatch.Stop();

            await SaveSearchHistoryAsync(criteria.Query, SearchEntityType.Category);

            return new SearchResult<Category>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                SearchTime = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error searching categories: {criteria.Query}");
            throw;
        }
    }

    public async Task<SearchResult<WarehouseApp.Core.Models.Location>> SearchLocationsAsync(SearchCriteria criteria)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation($"Searching locations with query: {criteria.Query}");

            var query = _unitOfWork.Locations.GetQueryable()
                .Include(l => l.Parent)
                .Include(l => l.Children)
                .AsQueryable();

            // Apply text search
            if (!string.IsNullOrWhiteSpace(criteria.Query))
            {
                var searchTerm = criteria.Query.ToLower();
                query = query.Where(l =>
                    l.Name.ToLower().Contains(searchTerm) ||
                    l.Description.ToLower().Contains(searchTerm) ||
                    l.Code.ToLower().Contains(searchTerm) ||
                    (l.Parent != null && l.Parent.Name.ToLower().Contains(searchTerm)));
            }

            // Apply filters
            query = ApplyLocationFilters(query, criteria.Filters);

            // Apply active/inactive filter
            if (!criteria.IncludeInactive)
            {
                query = query.Where(l => l.IsActive);
            }

            var totalCount = await query.CountAsync();
            query = ApplyLocationSorting(query, criteria.SortBy, criteria.SortDirection);

            var items = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            stopwatch.Stop();

            await SaveSearchHistoryAsync(criteria.Query, SearchEntityType.Location);

            return new SearchResult<WarehouseApp.Core.Models.Location>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                SearchTime = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error searching locations: {criteria.Query}");
            throw;
        }
    }

    public async Task<SearchResult<StockMovement>> SearchStockMovementsAsync(SearchCriteria criteria)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation($"Searching stock movements with query: {criteria.Query}");

            var query = _unitOfWork.StockMovements.GetQueryable()
                .Include(sm => sm.Product)
                .Include(sm => sm.Location)
                .Include(sm => sm.User)
                .AsQueryable();

            // Apply text search
            if (!string.IsNullOrWhiteSpace(criteria.Query))
            {
                var searchTerm = criteria.Query.ToLower();
                query = query.Where(sm =>
                    (sm.Product != null && sm.Product.Name.ToLower().Contains(searchTerm)) ||
                    (sm.Product != null && sm.Product.SKU.ToLower().Contains(searchTerm)) ||
                    (sm.Location != null && sm.Location.Name.ToLower().Contains(searchTerm)) ||
                    (sm.User != null && sm.User.Name.ToLower().Contains(searchTerm)) ||
                    sm.Notes.ToLower().Contains(searchTerm));
            }

            // Apply filters
            query = ApplyStockMovementFilters(query, criteria.Filters);

            var totalCount = await query.CountAsync();
            query = ApplyStockMovementSorting(query, criteria.SortBy, criteria.SortDirection);

            var items = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            stopwatch.Stop();

            await SaveSearchHistoryAsync(criteria.Query, SearchEntityType.StockMovement);

            return new SearchResult<StockMovement>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                SearchTime = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error searching stock movements: {criteria.Query}");
            throw;
        }
    }

    public async Task<IEnumerable<SearchSuggestion>> GetSearchSuggestionsAsync(string query, SearchEntityType entityType)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return new List<SearchSuggestion>();

            var suggestions = new List<SearchSuggestion>();
            var searchTerm = query.ToLower();

            switch (entityType)
            {
                case SearchEntityType.Product:
                    suggestions.AddRange(await GetProductSuggestions(searchTerm));
                    break;
                case SearchEntityType.Category:
                    suggestions.AddRange(await GetCategorySuggestions(searchTerm));
                    break;
                case SearchEntityType.Location:
                    suggestions.AddRange(await GetLocationSuggestions(searchTerm));
                    break;
                case SearchEntityType.All:
                    suggestions.AddRange(await GetProductSuggestions(searchTerm));
                    suggestions.AddRange(await GetCategorySuggestions(searchTerm));
                    suggestions.AddRange(await GetLocationSuggestions(searchTerm));
                    break;
            }

            return suggestions.OrderByDescending(s => s.Count).Take(10);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting search suggestions for: {query}");
            return new List<SearchSuggestion>();
        }
    }

    public async Task<SearchFilters> GetAvailableFiltersAsync(SearchEntityType entityType)
    {
        try
        {
            var filters = new SearchFilters();

            switch (entityType)
            {
                case SearchEntityType.Product:
                    filters = await GetProductFilters();
                    break;
                case SearchEntityType.Category:
                    filters = await GetCategoryFilters();
                    break;
                case SearchEntityType.Location:
                    filters = await GetLocationFilters();
                    break;
                case SearchEntityType.StockMovement:
                    filters = await GetStockMovementFilters();
                    break;
            }

            return filters;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting available filters for: {entityType}");
            return new SearchFilters();
        }
    }

    public async Task SaveSearchHistoryAsync(string query, SearchEntityType entityType)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return;

            // Remove existing entry if exists
            var existing = _searchHistory.FirstOrDefault(h =>
                h.Query.Equals(query, StringComparison.OrdinalIgnoreCase) &&
                h.EntityType == entityType);

            if (existing != null)
            {
                _searchHistory.Remove(existing);
            }

            // Add new entry at the beginning
            _searchHistory.Insert(0, new SearchHistoryItem
            {
                Query = query,
                EntityType = entityType,
                SearchedAt = DateTime.Now,
                ResultCount = 0 // Will be updated with actual count
            });

            // Keep only last 50 searches
            while (_searchHistory.Count > 50)
            {
                _searchHistory.RemoveAt(_searchHistory.Count - 1);
            }

            // TODO: Persist to local storage
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving search history: {query}");
        }
    }

    public async Task<IEnumerable<SearchHistoryItem>> GetSearchHistoryAsync(SearchEntityType? entityType = null)
    {
        try
        {
            var history = _searchHistory.AsEnumerable();

            if (entityType.HasValue)
            {
                history = history.Where(h => h.EntityType == entityType.Value);
            }

            return history.OrderByDescending(h => h.SearchedAt).Take(20);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting search history");
            return new List<SearchHistoryItem>();
        }
    }

    public async Task ClearSearchHistoryAsync()
    {
        try
        {
            _searchHistory.Clear();
            // TODO: Clear from local storage
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing search history");
        }
    }

    #region Private Helper Methods

    private IQueryable<Product> ApplyProductFilters(IQueryable<Product> query, Dictionary<string, object> filters)
    {
        foreach (var filter in filters)
        {
            switch (filter.Key.ToLower())
            {
                case "categoryid":
                    if (filter.Value is int categoryId)
                        query = query.Where(p => p.CategoryId == categoryId);
                    break;
                case "locationid":
                    if (filter.Value is int locationId)
                        query = query.Where(p => p.LocationId == locationId);
                    break;
                case "minprice":
                    if (filter.Value is decimal minPrice)
                        query = query.Where(p => p.Price >= minPrice);
                    break;
                case "maxprice":
                    if (filter.Value is decimal maxPrice)
                        query = query.Where(p => p.Price <= maxPrice);
                    break;
                case "minstock":
                    if (filter.Value is int minStock)
                        query = query.Where(p => p.CurrentStock >= minStock);
                    break;
                case "maxstock":
                    if (filter.Value is int maxStock)
                        query = query.Where(p => p.CurrentStock <= maxStock);
                    break;
                case "lowstock":
                    if (filter.Value is bool lowStock && lowStock)
                        query = query.Where(p => p.CurrentStock <= p.MinimumStock);
                    break;
                case "outofstock":
                    if (filter.Value is bool outOfStock && outOfStock)
                        query = query.Where(p => p.CurrentStock == 0);
                    break;
                case "createdfrom":
                    if (filter.Value is DateTime createdFrom)
                        query = query.Where(p => p.CreatedAt >= createdFrom);
                    break;
                case "createdto":
                    if (filter.Value is DateTime createdTo)
                        query = query.Where(p => p.CreatedAt <= createdTo);
                    break;
            }
        }
        return query;
    }

    private IQueryable<Product> ApplyProductSorting(IQueryable<Product> query, string sortBy, SortDirection direction)
    {
        return sortBy.ToLower() switch
        {
            "name" => direction == SortDirection.Ascending
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name),
            "sku" => direction == SortDirection.Ascending
                ? query.OrderBy(p => p.SKU)
                : query.OrderByDescending(p => p.SKU),
            "price" => direction == SortDirection.Ascending
                ? query.OrderBy(p => p.Price)
                : query.OrderByDescending(p => p.Price),
            "stock" => direction == SortDirection.Ascending
                ? query.OrderBy(p => p.CurrentStock)
                : query.OrderByDescending(p => p.CurrentStock),
            "created" => direction == SortDirection.Ascending
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt),
            "updated" => direction == SortDirection.Ascending
                ? query.OrderBy(p => p.UpdatedAt)
                : query.OrderByDescending(p => p.UpdatedAt),
            _ => query.OrderBy(p => p.Name)
        };
    }

    private IQueryable<Category> ApplyCategoryFilters(IQueryable<Category> query, Dictionary<string, object> filters)
    {
        foreach (var filter in filters)
        {
            switch (filter.Key.ToLower())
            {
                case "parentid":
                    if (filter.Value is int parentId)
                        query = query.Where(c => c.ParentId == parentId);
                    else if (filter.Value == null)
                        query = query.Where(c => c.ParentId == null);
                    break;
                case "haschildren":
                    if (filter.Value is bool hasChildren)
                        query = hasChildren
                            ? query.Where(c => c.Children.Any())
                            : query.Where(c => !c.Children.Any());
                    break;
                case "level":
                    if (filter.Value is int level)
                    {
                        // Filter by hierarchy level (root = 0, first level = 1, etc.)
                        if (level == 0)
                            query = query.Where(c => c.ParentId == null);
                        else
                        {
                            // This would need recursive logic for deeper levels
                            query = query.Where(c => c.ParentId != null);
                        }
                    }
                    break;
            }
        }
        return query;
    }

    private IQueryable<Category> ApplyCategorySorting(IQueryable<Category> query, string sortBy, SortDirection direction)
    {
        return sortBy.ToLower() switch
        {
            "name" => direction == SortDirection.Ascending
                ? query.OrderBy(c => c.Name)
                : query.OrderByDescending(c => c.Name),
            "created" => direction == SortDirection.Ascending
                ? query.OrderBy(c => c.CreatedAt)
                : query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Name)
        };
    }

    private IQueryable<WarehouseApp.Core.Models.Location> ApplyLocationFilters(IQueryable<WarehouseApp.Core.Models.Location> query, Dictionary<string, object> filters)
    {
        foreach (var filter in filters)
        {
            switch (filter.Key.ToLower())
            {
                case "parentid":
                    if (filter.Value is int parentId)
                        query = query.Where(l => l.ParentId == parentId);
                    else if (filter.Value == null)
                        query = query.Where(l => l.ParentId == null);
                    break;
                case "haschildren":
                    if (filter.Value is bool hasChildren)
                        query = hasChildren
                            ? query.Where(l => l.Children.Any())
                            : query.Where(l => !l.Children.Any());
                    break;
                case "type":
                    if (filter.Value is string locationType)
                        query = query.Where(l => l.Code.StartsWith(locationType));
                    break;
            }
        }
        return query;
    }

    private IQueryable<WarehouseApp.Core.Models.Location> ApplyLocationSorting(IQueryable<WarehouseApp.Core.Models.Location> query, string sortBy, SortDirection direction)
    {
        return sortBy.ToLower() switch
        {
            "name" => direction == SortDirection.Ascending
                ? query.OrderBy(l => l.Name)
                : query.OrderByDescending(l => l.Name),
            "code" => direction == SortDirection.Ascending
                ? query.OrderBy(l => l.Code)
                : query.OrderByDescending(l => l.Code),
            "created" => direction == SortDirection.Ascending
                ? query.OrderBy(l => l.CreatedAt)
                : query.OrderByDescending(l => l.CreatedAt),
            _ => query.OrderBy(l => l.Name)
        };
    }

    private IQueryable<StockMovement> ApplyStockMovementFilters(IQueryable<StockMovement> query, Dictionary<string, object> filters)
    {
        foreach (var filter in filters)
        {
            switch (filter.Key.ToLower())
            {
                case "productid":
                    if (filter.Value is int productId)
                        query = query.Where(sm => sm.ProductId == productId);
                    break;
                case "locationid":
                    if (filter.Value is int locationId)
                        query = query.Where(sm => sm.LocationId == locationId);
                    break;
                case "movementtype":
                    if (filter.Value is MovementType movementType)
                        query = query.Where(sm => sm.MovementType == movementType);
                    break;
                case "datefrom":
                    if (filter.Value is DateTime dateFrom)
                        query = query.Where(sm => sm.MovementDate >= dateFrom);
                    break;
                case "dateto":
                    if (filter.Value is DateTime dateTo)
                        query = query.Where(sm => sm.MovementDate <= dateTo);
                    break;
                case "userid":
                    if (filter.Value is string userId)
                        query = query.Where(sm => sm.UserId == userId);
                    break;
            }
        }
        return query;
    }

    private IQueryable<StockMovement> ApplyStockMovementSorting(IQueryable<StockMovement> query, string sortBy, SortDirection direction)
    {
        return sortBy.ToLower() switch
        {
            "date" => direction == SortDirection.Ascending
                ? query.OrderBy(sm => sm.MovementDate)
                : query.OrderByDescending(sm => sm.MovementDate),
            "quantity" => direction == SortDirection.Ascending
                ? query.OrderBy(sm => sm.Quantity)
                : query.OrderByDescending(sm => sm.Quantity),
            "type" => direction == SortDirection.Ascending
                ? query.OrderBy(sm => sm.MovementType)
                : query.OrderByDescending(sm => sm.MovementType),
            _ => query.OrderByDescending(sm => sm.MovementDate)
        };
    }

    private async Task<IEnumerable<SearchSuggestion>> GetProductSuggestions(string searchTerm)
    {
        var suggestions = new List<SearchSuggestion>();

        // Product names
        var productNames = await _unitOfWork.Products.GetQueryable()
            .Where(p => p.Name.ToLower().Contains(searchTerm) && p.IsActive)
            .Select(p => p.Name)
            .Distinct()
            .Take(5)
            .ToListAsync();

        suggestions.AddRange(productNames.Select(name => new SearchSuggestion
        {
            Text = name,
            Category = "Product Name",
            EntityType = SearchEntityType.Product,
            Count = 1
        }));

        // SKUs
        var skus = await _unitOfWork.Products.GetQueryable()
            .Where(p => p.SKU.ToLower().Contains(searchTerm) && p.IsActive)
            .Select(p => p.SKU)
            .Distinct()
            .Take(3)
            .ToListAsync();

        suggestions.AddRange(skus.Select(sku => new SearchSuggestion
        {
            Text = sku,
            Category = "SKU",
            EntityType = SearchEntityType.Product,
            Count = 1
        }));

        return suggestions;
    }

    private async Task<IEnumerable<SearchSuggestion>> GetCategorySuggestions(string searchTerm)
    {
        var categoryNames = await _unitOfWork.Categories.GetQueryable()
            .Where(c => c.Name.ToLower().Contains(searchTerm) && c.IsActive)
            .Select(c => c.Name)
            .Distinct()
            .Take(5)
            .ToListAsync();

        return categoryNames.Select(name => new SearchSuggestion
        {
            Text = name,
            Category = "Category",
            EntityType = SearchEntityType.Category,
            Count = 1
        });
    }

    private async Task<IEnumerable<SearchSuggestion>> GetLocationSuggestions(string searchTerm)
    {
        var locationNames = await _unitOfWork.Locations.GetQueryable()
            .Where(l => l.Name.ToLower().Contains(searchTerm) && l.IsActive)
            .Select(l => l.Name)
            .Distinct()
            .Take(5)
            .ToListAsync();

        return locationNames.Select(name => new SearchSuggestion
        {
            Text = name,
            Category = "Location",
            EntityType = SearchEntityType.Location,
            Count = 1
        });
    }

    private async Task<Dictionary<string, int>> GenerateProductFacets(SearchCriteria criteria)
    {
        var facets = new Dictionary<string, int>();

        try
        {
            var baseQuery = _unitOfWork.Products.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Location)
                .AsQueryable();

            // Apply text search but not other filters for facet generation
            if (!string.IsNullOrWhiteSpace(criteria.Query))
            {
                var searchTerm = criteria.Query.ToLower();
                baseQuery = baseQuery.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.SKU.ToLower().Contains(searchTerm) ||
                    p.Barcode.ToLower().Contains(searchTerm));
            }

            // Category facets
            var categoryFacets = await baseQuery
                .Where(p => p.Category != null)
                .GroupBy(p => p.Category!.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => $"category:{x.Name}", x => x.Count);

            foreach (var facet in categoryFacets)
                facets[facet.Key] = facet.Value;

            // Location facets
            var locationFacets = await baseQuery
                .Where(p => p.Location != null)
                .GroupBy(p => p.Location!.Name)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => $"location:{x.Name}", x => x.Count);

            foreach (var facet in locationFacets)
                facets[facet.Key] = facet.Value;

            // Stock status facets
            var lowStockCount = await baseQuery.CountAsync(p => p.CurrentStock <= p.MinimumStock);
            var outOfStockCount = await baseQuery.CountAsync(p => p.CurrentStock == 0);

            if (lowStockCount > 0)
                facets["stock:low"] = lowStockCount;
            if (outOfStockCount > 0)
                facets["stock:out"] = outOfStockCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating product facets");
        }

        return facets;
    }

    private async Task<SearchFilters> GetProductFilters()
    {
        var filters = new SearchFilters();

        try
        {
            // Category filter
            var categories = await _unitOfWork.Categories.GetQueryable()
                .Where(c => c.IsActive)
                .Select(c => new FilterValue { Value = c.Id.ToString(), DisplayText = c.Name })
                .ToListAsync();

            filters.AvailableFilters["categoryId"] = new FilterOption
            {
                Name = "categoryId",
                DisplayName = "Category",
                Type = FilterType.Select,
                Values = categories
            };

            // Location filter
            var locations = await _unitOfWork.Locations.GetQueryable()
                .Where(l => l.IsActive)
                .Select(l => new FilterValue { Value = l.Id.ToString(), DisplayText = l.Name })
                .ToListAsync();

            filters.AvailableFilters["locationId"] = new FilterOption
            {
                Name = "locationId",
                DisplayName = "Location",
                Type = FilterType.Select,
                Values = locations
            };

            // Price range filter
            var priceStats = await _unitOfWork.Products.GetQueryable()
                .Where(p => p.IsActive)
                .Select(p => p.Price)
                .ToListAsync();

            if (priceStats.Any())
            {
                filters.AvailableFilters["priceRange"] = new FilterOption
                {
                    Name = "priceRange",
                    DisplayName = "Price Range",
                    Type = FilterType.Range,
                    MinValue = priceStats.Min(),
                    MaxValue = priceStats.Max()
                };
            }

            // Stock filters
            filters.AvailableFilters["lowStock"] = new FilterOption
            {
                Name = "lowStock",
                DisplayName = "Low Stock Only",
                Type = FilterType.Boolean
            };

            filters.AvailableFilters["outOfStock"] = new FilterOption
            {
                Name = "outOfStock",
                DisplayName = "Out of Stock Only",
                Type = FilterType.Boolean
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product filters");
        }

        return filters;
    }

    private async Task<SearchFilters> GetCategoryFilters()
    {
        var filters = new SearchFilters();

        try
        {
            // Parent category filter
            var parentCategories = await _unitOfWork.Categories.GetQueryable()
                .Where(c => c.IsActive && c.ParentId == null)
                .Select(c => new FilterValue { Value = c.Id.ToString(), DisplayText = c.Name })
                .ToListAsync();

            filters.AvailableFilters["parentId"] = new FilterOption
            {
                Name = "parentId",
                DisplayName = "Parent Category",
                Type = FilterType.Select,
                Values = parentCategories
            };

            // Has children filter
            filters.AvailableFilters["hasChildren"] = new FilterOption
            {
                Name = "hasChildren",
                DisplayName = "Has Subcategories",
                Type = FilterType.Boolean
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category filters");
        }

        return filters;
    }

    private async Task<SearchFilters> GetLocationFilters()
    {
        var filters = new SearchFilters();

        try
        {
            // Parent location filter
            var parentLocations = await _unitOfWork.Locations.GetQueryable()
                .Where(l => l.IsActive && l.ParentId == null)
                .Select(l => new FilterValue { Value = l.Id.ToString(), DisplayText = l.Name })
                .ToListAsync();

            filters.AvailableFilters["parentId"] = new FilterOption
            {
                Name = "parentId",
                DisplayName = "Parent Location",
                Type = FilterType.Select,
                Values = parentLocations
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location filters");
        }

        return filters;
    }

    private async Task<SearchFilters> GetStockMovementFilters()
    {
        var filters = new SearchFilters();

        try
        {
            // Movement type filter
            var movementTypes = Enum.GetValues<MovementType>()
                .Select(mt => new FilterValue
                {
                    Value = ((int)mt).ToString(),
                    DisplayText = mt.ToString()
                })
                .ToList();

            filters.AvailableFilters["movementType"] = new FilterOption
            {
                Name = "movementType",
                DisplayName = "Movement Type",
                Type = FilterType.Select,
                Values = movementTypes
            };

            // Date range filter
            filters.AvailableFilters["dateRange"] = new FilterOption
            {
                Name = "dateRange",
                DisplayName = "Date Range",
                Type = FilterType.Range,
                MinValue = DateTime.Now.AddMonths(-12),
                MaxValue = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock movement filters");
        }

        return filters;
    }

    #endregion
}