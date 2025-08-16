using WarehouseApp.MAUI.ViewModels;

namespace WarehouseApp.MAUI.Views;

public partial class ProductListPage : ContentPage
{
    public ProductListPage(ProductListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProductListViewModel viewModel)
        {
            viewModel.OnAppearing();
            await viewModel.InitializeAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is ProductListViewModel viewModel)
        {
            viewModel.OnDisappearing();
        }
    }
}