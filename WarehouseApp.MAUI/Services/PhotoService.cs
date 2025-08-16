using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.Logging;

namespace WarehouseApp.MAUI.Services;

public class PhotoService : IPhotoService
{
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(ILogger<PhotoService> logger)
    {
        _logger = logger;
    }

    public async Task<string?> CapturePhotoAsync(string? fileName = null)
    {
        try
        {
            // Check camera availability and permissions
            if (!await IsCameraAvailableAsync())
            {
                _logger.LogWarning("Camera is not available on this device");
                return null;
            }

            if (!await RequestCameraPermissionAsync())
            {
                _logger.LogWarning("Camera permission denied");
                return null;
            }

            // Generate filename if not provided
            fileName ??= $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";

            // Capture photo using MediaPicker
            var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Take a photo"
            });

            if (photo == null)
            {
                _logger.LogInformation("Photo capture was cancelled by user");
                return null;
            }

            // Save to app storage
            var savedPath = await SaveImageToAppStorageAsync(photo.FullPath, fileName);

            _logger.LogInformation($"Photo captured and saved to: {savedPath}");
            return savedPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error capturing photo");
            return null;
        }
    }

    public async Task<string?> PickPhotoAsync()
    {
        try
        {
            // Check gallery availability and permissions
            if (!await IsGalleryAvailableAsync())
            {
                _logger.LogWarning("Photo gallery is not available on this device");
                return null;
            }

            if (!await RequestGalleryPermissionAsync())
            {
                _logger.LogWarning("Gallery permission denied");
                return null;
            }

            // Pick photo from gallery
            var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select a photo"
            });

            if (photo == null)
            {
                _logger.LogInformation("Photo selection was cancelled by user");
                return null;
            }

            // Generate filename and save to app storage
            var fileName = $"picked_{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(photo.FileName)}";
            var savedPath = await SaveImageToAppStorageAsync(photo.FullPath, fileName);

            _logger.LogInformation($"Photo picked and saved to: {savedPath}");
            return savedPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error picking photo");
            return null;
        }
    }

    public async Task<string> CompressImageAsync(string imagePath, int quality = 80, int maxWidth = 1024, int maxHeight = 1024)
    {
        try
        {
            _logger.LogInformation($"Compressing image: {imagePath}");

            // For now, implement a simple file copy as compression placeholder
            // In a real implementation, you would use Microsoft.Maui.Graphics or similar
            var compressedPath = Path.Combine(
                Path.GetDirectoryName(imagePath)!,
                $"compressed_{Path.GetFileName(imagePath)}"
            );

            // Simple file copy for now
            File.Copy(imagePath, compressedPath, true);

            _logger.LogInformation($"Image compression placeholder - copied to: {compressedPath}");
            return compressedPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error compressing image: {imagePath}");
            throw;
        }
    }

    public async Task<string> SaveImageToAppStorageAsync(string imagePath, string fileName)
    {
        try
        {
            // Create images directory in app data
            var imagesDirectory = Path.Combine(FileSystem.AppDataDirectory, "Images");
            Directory.CreateDirectory(imagesDirectory);

            // Create target path
            var targetPath = Path.Combine(imagesDirectory, fileName);

            // Copy file to app storage
            using var sourceStream = File.OpenRead(imagePath);
            using var targetStream = File.Create(targetPath);
            await sourceStream.CopyToAsync(targetStream);

            _logger.LogInformation($"Image saved to app storage: {targetPath}");
            return targetPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving image to app storage: {imagePath}");
            throw;
        }
    }

    public async Task<bool> DeleteImageAsync(string imagePath)
    {
        try
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
                _logger.LogInformation($"Image deleted: {imagePath}");
                return true;
            }

            _logger.LogWarning($"Image not found for deletion: {imagePath}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting image: {imagePath}");
            return false;
        }
    }

    public async Task<(int width, int height)> GetImageDimensionsAsync(string imagePath)
    {
        try
        {
            // For now, return default dimensions
            // In a real implementation, you would use Microsoft.Maui.Graphics or similar
            return (1920, 1080); // Default placeholder dimensions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting image dimensions: {imagePath}");
            throw;
        }
    }

    public async Task<bool> IsCameraAvailableAsync()
    {
        try
        {
            return MediaPicker.Default.IsCaptureSupported;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking camera availability");
            return false;
        }
    }

    public async Task<bool> IsGalleryAvailableAsync()
    {
        try
        {
            // Gallery is generally available on all platforms
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking gallery availability");
            return false;
        }
    }

    public async Task<bool> RequestCameraPermissionAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting camera permission");
            return false;
        }
    }

    public async Task<bool> RequestGalleryPermissionAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Photos>();
            return status == PermissionStatus.Granted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting gallery permission");
            return false;
        }
    }

    private static (int width, int height) CalculateNewDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
    {
        // If image is already smaller than max dimensions, return original size
        if (originalWidth <= maxWidth && originalHeight <= maxHeight)
        {
            return (originalWidth, originalHeight);
        }

        // Calculate aspect ratio
        var aspectRatio = (double)originalWidth / originalHeight;

        int newWidth, newHeight;

        if (originalWidth > originalHeight)
        {
            // Landscape orientation
            newWidth = Math.Min(originalWidth, maxWidth);
            newHeight = (int)(newWidth / aspectRatio);

            if (newHeight > maxHeight)
            {
                newHeight = maxHeight;
                newWidth = (int)(newHeight * aspectRatio);
            }
        }
        else
        {
            // Portrait orientation
            newHeight = Math.Min(originalHeight, maxHeight);
            newWidth = (int)(newHeight * aspectRatio);

            if (newWidth > maxWidth)
            {
                newWidth = maxWidth;
                newHeight = (int)(newWidth / aspectRatio);
            }
        }

        return (newWidth, newHeight);
    }
}