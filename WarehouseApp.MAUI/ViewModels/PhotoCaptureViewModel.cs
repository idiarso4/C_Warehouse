using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WarehouseApp.MAUI.Services;

namespace WarehouseApp.MAUI.ViewModels;

public partial class PhotoCaptureViewModel : BaseViewModel
{
    private readonly IPhotoService _photoService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string? _selectedImagePath;

    [ObservableProperty]
    private bool _isCameraAvailable;

    [ObservableProperty]
    private bool _isGalleryAvailable;

    [ObservableProperty]
    private ObservableCollection<string> _capturedImages = new();

    [ObservableProperty]
    private string? _imagePreviewPath;

    [ObservableProperty]
    private bool _showImagePreview;

    [ObservableProperty]
    private string _compressionQuality = "80";

    [ObservableProperty]
    private string _maxImageWidth = "1024";

    [ObservableProperty]
    private string _maxImageHeight = "1024";

    public PhotoCaptureViewModel(
        IPhotoService photoService,
        INavigationService navigationService)
    {
        _photoService = photoService;
        _navigationService = navigationService;

        Title = "Photo Capture";
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await CheckDeviceCapabilities();
        await LoadExistingImages();
    }

    [RelayCommand]
    private async Task CapturePhotoAsync()
    {
        if (IsBusy) return;

        await SetBusyAsync(async () =>
        {
            try
            {
                var imagePath = await _photoService.CapturePhotoAsync();

                if (!string.IsNullOrEmpty(imagePath))
                {
                    // Add to captured images collection
                    CapturedImages.Insert(0, imagePath);

                    // Set as selected image
                    SelectedImagePath = imagePath;

                    // Show preview
                    await DisplayImagePreview(imagePath);

                    await ShowSuccessAsync("Photo captured successfully!");
                }
                else
                {
                    await ShowInfoAsync("Photo capture was cancelled.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error capturing photo: {ex.Message}");
            }
        }, "Capturing photo...");
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        if (IsBusy) return;

        await SetBusyAsync(async () =>
        {
            try
            {
                var imagePath = await _photoService.PickPhotoAsync();

                if (!string.IsNullOrEmpty(imagePath))
                {
                    // Add to captured images collection
                    CapturedImages.Insert(0, imagePath);

                    // Set as selected image
                    SelectedImagePath = imagePath;

                    // Show preview
                    await DisplayImagePreview(imagePath);

                    await ShowSuccessAsync("Photo selected successfully!");
                }
                else
                {
                    await ShowInfoAsync("Photo selection was cancelled.");
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error selecting photo: {ex.Message}");
            }
        }, "Selecting photo...");
    }

    [RelayCommand]
    private async Task CompressImageAsync(string imagePath)
    {
        if (IsBusy || string.IsNullOrEmpty(imagePath)) return;

        await SetBusyAsync(async () =>
        {
            try
            {
                // Parse compression settings
                if (!int.TryParse(CompressionQuality, out var quality))
                    quality = 80;

                if (!int.TryParse(MaxImageWidth, out var maxWidth))
                    maxWidth = 1024;

                if (!int.TryParse(MaxImageHeight, out var maxHeight))
                    maxHeight = 1024;

                // Validate ranges
                quality = Math.Clamp(quality, 10, 100);
                maxWidth = Math.Clamp(maxWidth, 100, 4096);
                maxHeight = Math.Clamp(maxHeight, 100, 4096);

                var compressedPath = await _photoService.CompressImageAsync(
                    imagePath, quality, maxWidth, maxHeight);

                // Add compressed image to collection
                CapturedImages.Insert(0, compressedPath);

                // Set as selected image
                SelectedImagePath = compressedPath;

                // Show preview
                await DisplayImagePreview(compressedPath);

                await ShowSuccessAsync("Image compressed successfully!");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Error compressing image: {ex.Message}");
            }
        }, "Compressing image...");
    }

    [RelayCommand]
    private async Task DeleteImageAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath)) return;

        var confirmed = await ShowConfirmAsync(
            "Delete Image",
            "Are you sure you want to delete this image?");

        if (!confirmed) return;

        try
        {
            var deleted = await _photoService.DeleteImageAsync(imagePath);

            if (deleted)
            {
                CapturedImages.Remove(imagePath);

                if (SelectedImagePath == imagePath)
                {
                    SelectedImagePath = CapturedImages.FirstOrDefault();
                    if (!string.IsNullOrEmpty(SelectedImagePath))
                    {
                        await DisplayImagePreview(SelectedImagePath);
                    }
                    else
                    {
                        HideImagePreview();
                    }
                }

                await ShowSuccessAsync("Image deleted successfully!");
            }
            else
            {
                await ShowErrorAsync("Failed to delete image.");
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error deleting image: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SelectImageAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath)) return;

        SelectedImagePath = imagePath;
        await DisplayImagePreview(imagePath);
    }

    [RelayCommand]
    private async Task GetImageInfoAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath)) return;

        try
        {
            var (width, height) = await _photoService.GetImageDimensionsAsync(imagePath);
            var fileInfo = new FileInfo(imagePath);
            var sizeKB = fileInfo.Length / 1024;

            await ShowInfoAsync($"Image Info:\nDimensions: {width} x {height}\nFile Size: {sizeKB:N0} KB\nPath: {Path.GetFileName(imagePath)}");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error getting image info: {ex.Message}");
        }
    }

    [RelayCommand]
    private void HideImagePreview()
    {
        ShowImagePreview = false;
        ImagePreviewPath = null;
    }

    [RelayCommand]
    private async Task ClearAllImagesAsync()
    {
        var confirmed = await ShowConfirmAsync(
            "Clear All Images",
            "Are you sure you want to delete all captured images?");

        if (!confirmed) return;

        try
        {
            var imagesToDelete = CapturedImages.ToList();
            var deletedCount = 0;

            foreach (var imagePath in imagesToDelete)
            {
                var deleted = await _photoService.DeleteImageAsync(imagePath);
                if (deleted)
                {
                    deletedCount++;
                }
            }

            CapturedImages.Clear();
            SelectedImagePath = null;
            HideImagePreview();

            await ShowSuccessAsync($"Deleted {deletedCount} images successfully!");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error clearing images: {ex.Message}");
        }
    }

    private async Task CheckDeviceCapabilities()
    {
        try
        {
            IsCameraAvailable = await _photoService.IsCameraAvailableAsync();
            IsGalleryAvailable = await _photoService.IsGalleryAvailableAsync();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error checking device capabilities: {ex.Message}");
        }
    }

    private async Task LoadExistingImages()
    {
        try
        {
            var imagesDirectory = Path.Combine(FileSystem.AppDataDirectory, "Images");

            if (!Directory.Exists(imagesDirectory))
                return;

            var imageFiles = Directory.GetFiles(imagesDirectory, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => IsImageFile(file))
                .OrderByDescending(File.GetCreationTime)
                .ToList();

            CapturedImages.Clear();
            foreach (var imageFile in imageFiles)
            {
                CapturedImages.Add(imageFile);
            }

            // Select first image if available
            if (CapturedImages.Any())
            {
                SelectedImagePath = CapturedImages.First();
                await DisplayImagePreview(SelectedImagePath);
            }
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error loading existing images: {ex.Message}");
        }
    }

    private async Task DisplayImagePreview(string imagePath)
    {
        try
        {
            ImagePreviewPath = imagePath;
            ShowImagePreview = true;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Error showing image preview: {ex.Message}");
        }
    }

    private static bool IsImageFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".webp";
    }

    partial void OnSelectedImagePathChanged(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            _ = DisplayImagePreview(value);
        }
    }
}