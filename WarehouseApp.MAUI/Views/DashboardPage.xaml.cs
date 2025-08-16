using WarehouseApp.MAUI.ViewModels;

namespace WarehouseApp.MAUI.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainViewModel viewModel)
        {
            viewModel.OnAppearing();
            await viewModel.InitializeAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is MainViewModel viewModel)
        {
            viewModel.OnDisappearing();
        }
    }
}