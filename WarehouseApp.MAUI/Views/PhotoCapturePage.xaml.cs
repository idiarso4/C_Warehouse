using WarehouseApp.MAUI.ViewModels;

namespace WarehouseApp.MAUI.Views;

public partial class PhotoCapturePage : ContentPage
{
    public PhotoCapturePage(PhotoCaptureViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is PhotoCaptureViewModel viewModel)
        {
            viewModel.OnAppearing();
            await viewModel.InitializeAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is PhotoCaptureViewModel viewModel)
        {
            viewModel.OnDisappearing();
        }
    }
}