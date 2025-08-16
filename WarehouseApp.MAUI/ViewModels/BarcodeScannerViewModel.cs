using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WarehouseApp.MAUI.Services;
using ZXing.Net.Maui;

namespace WarehouseApp.MAUI.ViewModels;

public partial class BarcodeScannerViewModel : BaseViewModel
{
    private readonly IBarcodeService _barcodeService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private bool _isScanning = true;

    [ObservableProperty]
    private bool _isTorchOn = false;

    [ObservableProperty]
    private string? _lastScannedValue;

    [ObservableProperty]
    private BarcodeFormat _lastScannedFormat;

    [ObservableProperty]
    private DateTime _lastScannedTime;

    [ObservableProperty]
    private ObservableCollection<WarehouseApp.MAUI.Services.BarcodeResult> _scanHistory = new();

    [ObservableProperty]
    private bool _showScanHistory = false;

    [ObservableProperty]
    private bool _continuousScanning = false;

    [ObservableProperty]
    private string _scanInstructions = "Point camera at barcode or QR code";

    [ObservableProperty]
    private BarcodeDetectionEventArgs? _lastDetection;

    public BarcodeScannerViewModel(
        IBarcodeService barcodeService,
        INavigationService navigationService)
    {
        _barcodeService = barcodeService;
        _navigationService = navigationService;

        Title = "Barcode Scanner";
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await CheckCameraPermission();
    }

    [RelayCommand]
    public async Task OnBarcodeDetectedAsync(BarcodeDetectionEventArgs e)
    {
        if (!IsScanning || e.Results?.Any() != true)
            return;

        try
        {
            var result = e.Results.First();

            // Update last scanned properties
            LastScannedValue = result.Value;
            LastScannedFormat = result.Format;
            LastScannedTime = DateTime.Now;
            LastDetection = e;

            // Create barcode result
            var barcodeResult = new WarehouseApp.MAUI.Services.BarcodeResult
            {
                Value = result.Value,
                Format = result.Format,
                ScannedAt = DateTime.Now
            };

            // Add to scan history
            ScanHistory.Insert(0, barcodeResult);

            // Limit history to 50 items
            while (ScanHistory.Count > 50)
            {
                ScanHistory.RemoveAt(ScanHistory.Count - 1);
            }

            // Stop scanning if not continuous
            if (!ContinuousScanning)
            {
                IsScanning = false;
                ScanInstructions = "Scan completed. Tap to scan again.";
            }

            // Show success feedback
            await ShowSuccessAsync($"Scanned: {result.Format} - {result.Value}");

            // Vibrate for feedback
            try
            {
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
            }
            catch
            {
                // Ignore vibration errors
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error processing barcode: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ToggleTorch()
    {
        IsTorchOn = !IsTorchOn;
    }

    [RelayCommand]
    private void ToggleScanning()
    {
        IsScanning = !IsScanning;
        ScanInstructions = IsScanning
            ? "Point camera at barcode or QR code"
            : "Scanning paused. Tap to resume.";
    }

    [RelayCommand]
    private void ToggleContinuousScanning()
    {
        ContinuousScanning = !ContinuousScanning;

        if (ContinuousScanning)
        {
            ScanInstructions = "Continuous scanning enabled";
        }
        else
        {
            ScanInstructions = "Single scan mode";
        }
    }

    [RelayCommand]
    private void ToggleScanHistory()
    {
        ShowScanHistory = !ShowScanHistory;
    }

    [RelayCommand]
    private async Task CopyToClipboardAsync(string value)
    {
        if (string.IsNullOrEmpty(value)) return;

        try
        {
            await Clipboard.Default.SetTextAsync(value);
            await ShowSuccessAsync("Copied to clipboard!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Failed to copy: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ShareBarcodeAsync(WarehouseApp.MAUI.Services.BarcodeResult barcode)
    {
        if (barcode == null) return;

        try
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = barcode.Value,
                Title = $"Share {barcode.Format} Code"
            });
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Failed to share: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SearchProductAsync(string barcode)
    {
        if (string.IsNullOrEmpty(barcode)) return;

        try
        {
            // Navigate to product search with barcode
            var parameters = new Dictionary<string, object>
            {
                ["searchQuery"] = barcode,
                ["searchType"] = "barcode"
            };

            await _navigationService.GoToAsync("products", parameters);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Failed to search product: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ClearScanHistory()
    {
        ScanHistory.Clear();
        ShowScanHistory = false;
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await _navigationService.GoBackAsync();
    }

    private async Task CheckCameraPermission()
    {
        try
        {
            var hasPermission = await _barcodeService.HasCameraPermissionAsync();

            if (!hasPermission)
            {
                var granted = await _barcodeService.RequestCameraPermissionAsync();

                if (!granted)
                {
                    await ShowErrorAsync("Camera permission is required for barcode scanning.");
                    await _navigationService.GoBackAsync();
                    return;
                }
            }

            IsScanning = true;
            ScanInstructions = "Point camera at barcode or QR code";
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error checking camera permission: {ex.Message}");
            await _navigationService.GoBackAsync();
        }
    }

    partial void OnIsScanningChanged(bool value)
    {
        if (value)
        {
            ScanInstructions = "Point camera at barcode or QR code";
        }
        else
        {
            ScanInstructions = "Scanning paused. Tap to resume.";
        }
    }
}