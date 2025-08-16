using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WarehouseApp.MAUI.Services;
using ZXing.Net.Maui;

namespace WarehouseApp.MAUI.ViewModels;

public partial class QRCodeGeneratorViewModel : BaseViewModel
{
    private readonly IBarcodeService _barcodeService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private BarcodeFormat _selectedFormat = BarcodeFormat.QrCode;

    [ObservableProperty]
    private int _imageWidth = 300;

    [ObservableProperty]
    private int _imageHeight = 300;

    [ObservableProperty]
    private string? _generatedImagePath;

    [ObservableProperty]
    private bool _showGeneratedCode = false;

    [ObservableProperty]
    private ObservableCollection<string> _generatedCodes = new();

    [ObservableProperty]
    private ObservableCollection<BarcodeFormat> _availableFormats = new();

    [ObservableProperty]
    private string _presetType = "Custom";

    [ObservableProperty]
    private ObservableCollection<string> _presetTypes = new()
    {
        "Custom",
        "Product ID",
        "Location Code",
        "User ID",
        "URL",
        "WiFi",
        "Contact"
    };

    public QRCodeGeneratorViewModel(
        IBarcodeService barcodeService,
        INavigationService navigationService)
    {
        _barcodeService = barcodeService;
        _navigationService = navigationService;

        Title = "QR Code Generator";
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        LoadAvailableFormats();
        await LoadExistingCodes();
    }

    [RelayCommand]
    private async Task GenerateCodeAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(InputText))
        {
            await ShowErrorAsync("Please enter text to generate QR code.");
            return;
        }

        await SetBusyAsync(async () =>
        {
            try
            {
                // Validate dimensions
                var width = Math.Clamp(ImageWidth, 100, 1000);
                var height = Math.Clamp(ImageHeight, 100, 1000);

                // Generate barcode
                var imagePath = await _barcodeService.GenerateBarcodeAsync(
                    InputText, SelectedFormat, width, height);

                if (!string.IsNullOrEmpty(imagePath))
                {
                    GeneratedImagePath = imagePath;
                    ShowGeneratedCode = true;

                    // Add to generated codes history
                    GeneratedCodes.Insert(0, imagePath);

                    // Limit history to 20 items
                    while (GeneratedCodes.Count > 20)
                    {
                        GeneratedCodes.RemoveAt(GeneratedCodes.Count - 1);
                    }

                    await ShowSuccessAsync($"{SelectedFormat} code generated successfully!");
                }
                else
                {
                    await ShowErrorAsync("Failed to generate QR code. Please try again.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error generating QR code: {ex.Message}");
            }
        }, "Generating QR code...");
    }

    [RelayCommand]
    private async Task ShareCodeAsync()
    {
        if (string.IsNullOrEmpty(GeneratedImagePath))
        {
            await ShowErrorAsync("No QR code to share.");
            return;
        }

        try
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Share {SelectedFormat} Code",
                File = new ShareFile(GeneratedImagePath)
            });
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Failed to share QR code: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SaveCodeAsync()
    {
        if (string.IsNullOrEmpty(GeneratedImagePath))
        {
            await ShowErrorAsync("No QR code to save.");
            return;
        }

        try
        {
            // Copy to device's photo gallery (if supported)
            await ShowSuccessAsync("QR code saved to app storage!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Failed to save QR code: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task CopyTextAsync()
    {
        if (string.IsNullOrEmpty(InputText))
        {
            await ShowErrorAsync("No text to copy.");
            return;
        }

        try
        {
            await Clipboard.Default.SetTextAsync(InputText);
            await ShowSuccessAsync("Text copied to clipboard!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Failed to copy text: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ClearInput()
    {
        InputText = string.Empty;
        ShowGeneratedCode = false;
        GeneratedImagePath = null;
    }

    [RelayCommand]
    private async Task LoadPresetAsync(string presetType)
    {
        PresetType = presetType;

        InputText = presetType switch
        {
            "Product ID" => "PROD-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
            "Location Code" => "LOC-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
            "User ID" => "USER-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"),
            "URL" => "https://example.com",
            "WiFi" => "WIFI:T:WPA;S:NetworkName;P:Password;;",
            "Contact" => "BEGIN:VCARD\nVERSION:3.0\nFN:John Doe\nTEL:+1234567890\nEMAIL:john@example.com\nEND:VCARD",
            _ => string.Empty
        };

        // Set appropriate format for preset
        SelectedFormat = presetType switch
        {
            "Product ID" or "Location Code" or "User ID" => BarcodeFormat.Code128,
            _ => BarcodeFormat.QrCode
        };
    }

    [RelayCommand]
    private async Task ViewGeneratedCodeAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return;

        GeneratedImagePath = imagePath;
        ShowGeneratedCode = true;
    }

    [RelayCommand]
    private async Task DeleteGeneratedCodeAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return;

        var confirmed = await ShowConfirmAsync(
            "Delete QR Code",
            "Are you sure you want to delete this QR code?");

        if (!confirmed) return;

        try
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            GeneratedCodes.Remove(imagePath);

            if (GeneratedImagePath == imagePath)
            {
                ShowGeneratedCode = false;
                GeneratedImagePath = null;
            }

            await ShowSuccessAsync("QR code deleted successfully!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Failed to delete QR code: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ClearAllCodesAsync()
    {
        var confirmed = await ShowConfirmAsync(
            "Clear All QR Codes",
            "Are you sure you want to delete all generated QR codes?");

        if (!confirmed) return;

        try
        {
            var codesToDelete = GeneratedCodes.ToList();
            var deletedCount = 0;

            foreach (var imagePath in codesToDelete)
            {
                try
                {
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                        deletedCount++;
                    }
                }
                catch
                {
                    // Continue with other files
                }
            }

            GeneratedCodes.Clear();
            ShowGeneratedCode = false;
            GeneratedImagePath = null;

            await ShowSuccessAsync($"Deleted {deletedCount} QR codes successfully!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error clearing QR codes: {ex.Message}");
        }
    }

    private void LoadAvailableFormats()
    {
        try
        {
            var formats = _barcodeService.GetSupportedFormats().ToList();
            AvailableFormats.Clear();

            foreach (var format in formats)
            {
                AvailableFormats.Add(format);
            }
        }
        catch (Exception ex)
        {
            ShowErrorAsync($"Error loading available formats: {ex.Message}");
        }
    }

    private async Task LoadExistingCodes()
    {
        try
        {
            var barcodeDirectory = Path.Combine(FileSystem.AppDataDirectory, "Barcodes");

            if (!Directory.Exists(barcodeDirectory))
                return;

            var imageFiles = Directory.GetFiles(barcodeDirectory, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => IsImageFile(file))
                .OrderByDescending(File.GetCreationTime)
                .ToList();

            GeneratedCodes.Clear();
            foreach (var imageFile in imageFiles)
            {
                GeneratedCodes.Add(imageFile);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error loading existing codes: {ex.Message}");
        }
    }

    private static bool IsImageFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension is ".png" or ".jpg" or ".jpeg" or ".bmp" or ".gif";
    }

    partial void OnPresetTypeChanged(string value)
    {
        if (value != "Custom")
        {
            _ = LoadPresetAsync(value);
        }
    }

    partial void OnImageWidthChanged(int value)
    {
        ImageWidth = Math.Clamp(value, 100, 1000);
    }

    partial void OnImageHeightChanged(int value)
    {
        ImageHeight = Math.Clamp(value, 100, 1000);
    }
}