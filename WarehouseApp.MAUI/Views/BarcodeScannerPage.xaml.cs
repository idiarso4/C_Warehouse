using WarehouseApp.MAUI.ViewModels;
using ZXing.Net.Maui;

namespace WarehouseApp.MAUI.Views;

public partial class BarcodeScannerPage : ContentPage
{
    public BarcodeScannerPage(BarcodeScannerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is BarcodeScannerViewModel viewModel)
        {
            viewModel.OnAppearing();
            await viewModel.InitializeAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is BarcodeScannerViewModel viewModel)
        {
            viewModel.OnDisappearing();
        }
    }

    private async void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (BindingContext is BarcodeScannerViewModel viewModel)
        {
            await viewModel.OnBarcodeDetectedAsync(e);
        }
    }
}