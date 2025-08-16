using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WarehouseApp.Core.DTOs;
using WarehouseApp.Core.Interfaces;
using WarehouseApp.MAUI.Services;

namespace WarehouseApp.MAUI.ViewModels;

public partial class ProductListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IProductService _productService;

    [ObservableProperty]
    private ObservableCollection<ProductDto> _products = new();

    [ObservableProperty]
    private ProductDto? _selectedProduct;

    [ObservableProperty]
    private bool _isGridView = false;

    [ObservableProperty]
    private string _sortBy = "Name";

    [ObservableProperty]
    private bool _sortAscending = true;

    public ProductListViewModel(
        INavigationService navigationService,
        IProductService productService)
    {
        _navigationService = navigationService;
        _productService = productService;

        Title = "Products";
    }

    public override async Task InitializeAsync()
    {
        await SetBusyAsync(LoadProductsAsync, "Loading products...");
    }

    protected override async Task LoadDataAsync()
    {
        await LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            // Sample data - replace with real service call
            var sampleProducts = new List<ProductDto>
            {
                new() { Id = 1, Name = "Laptop Dell XPS 13", SKU = "DELL-XPS-001", Price = 1299.99m, CurrentStock = 15, CategoryName = "Electronics" },
                new() { Id = 2, Name = "Wireless Mouse", SKU = "MOUSE-001", Price = 29.99m, CurrentStock = 50, CategoryName = "Electronics" },
                new() { Id = 3, Name = "Office Chair", SKU = "CHAIR-001", Price = 199.99m, CurrentStock = 8, CategoryName = "Furniture" },
                new() { Id = 4, Name = "Notebook A4", SKU = "NOTE-001", Price = 2.99m, CurrentStock = 100, CategoryName = "Stationery" },
                new() { Id = 5, Name = "USB Cable", SKU = "USB-001", Price = 9.99m, CurrentStock = 25, CategoryName = "Electronics" }
            };

            Products.Clear();
            foreach (var product in sampleProducts)
            {
                Products.Add(product);
            }

            IsEmpty = !Products.Any();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected override async Task PerformSearchAsync(string searchTerm)
    {
        await SetBusyAsync(async () =>
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                await LoadProductsAsync();
                return;
            }

            // Filter products based on search term
            var filteredProducts = Products
                .Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           p.SKU.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Products.Clear();
            foreach (var product in filteredProducts)
            {
                Products.Add(product);
            }

            IsEmpty = !Products.Any();
        }, "Searching...");
    }

    [RelayCommand]
    private async Task AddProductAsync()
    {
        await _navigationService.NavigateToAsync("//products/add");
    }

    [RelayCommand]
    private async Task EditProductAsync(ProductDto? product)
    {
        if (product != null)
        {
            var parameters = new Dictionary<string, object>
            {
                ["ProductId"] = product.Id
            };
            await _navigationService.NavigateToAsync("//products/edit", parameters);
        }
    }

    [RelayCommand]
    private async Task DeleteProductAsync(ProductDto? product)
    {
        if (product == null) return;

        var confirmed = await ShowConfirmAsync(
            "Delete Product",
            $"Are you sure you want to delete '{product.Name}'?");

        if (confirmed)
        {
            await SetBusyAsync(async () =>
            {
                // Delete product logic here
                Products.Remove(product);
                await ShowSuccessAsync("Product deleted successfully");
            }, "Deleting product...");
        }
    }

    [RelayCommand]
    private async Task ViewProductDetailsAsync(ProductDto? product)
    {
        if (product != null)
        {
            var parameters = new Dictionary<string, object>
            {
                ["ProductId"] = product.Id
            };
            await _navigationService.NavigateToAsync("//products/details", parameters);
        }
    }

    [RelayCommand]
    private void ToggleViewMode()
    {
        IsGridView = !IsGridView;
    }

    [RelayCommand]
    private async Task SortProductsAsync(string sortBy)
    {
        if (SortBy == sortBy)
        {
            SortAscending = !SortAscending;
        }
        else
        {
            SortBy = sortBy;
            SortAscending = true;
        }

        await ApplySortingAsync();
    }

    private async Task ApplySortingAsync()
    {
        await SetBusyAsync(() =>
        {
            var sortedProducts = SortBy switch
            {
                "Name" => SortAscending
                    ? Products.OrderBy(p => p.Name).ToList()
                    : Products.OrderByDescending(p => p.Name).ToList(),
                "Price" => SortAscending
                    ? Products.OrderBy(p => p.Price).ToList()
                    : Products.OrderByDescending(p => p.Price).ToList(),
                "Stock" => SortAscending
                    ? Products.OrderBy(p => p.CurrentStock).ToList()
                    : Products.OrderByDescending(p => p.CurrentStock).ToList(),
                _ => Products.ToList()
            };

            Products.Clear();
            foreach (var product in sortedProducts)
            {
                Products.Add(product);
            }

            return Task.CompletedTask;
        }, "Sorting...");
    }

    partial void OnSelectedProductChanged(ProductDto? value)
    {
        if (value != null)
        {
            // Handle selection
            _ = ViewProductDetailsAsync(value);
        }
    }
}