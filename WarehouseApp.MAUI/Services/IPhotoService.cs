namespace WarehouseApp.MAUI.Services;

public interface IPhotoService
{
    /// <summary>
    /// Capture a photo using the device camera
    /// </summary>
    /// <param name="fileName">Optional custom filename</param>
    /// <returns>Path to the captured photo or null if cancelled</returns>
    Task<string?> CapturePhotoAsync(string? fileName = null);

    /// <summary>
    /// Pick a photo from the device gallery
    /// </summary>
    /// <returns>Path to the selected photo or null if cancelled</returns>
    Task<string?> PickPhotoAsync();

    /// <summary>
    /// Compress an image to reduce file size
    /// </summary>
    /// <param name="imagePath">Path to the original image</param>
    /// <param name="quality">Compression quality (0-100)</param>
    /// <param name="maxWidth">Maximum width in pixels</param>
    /// <param name="maxHeight">Maximum height in pixels</param>
    /// <returns>Path to the compressed image</returns>
    Task<string> CompressImageAsync(string imagePath, int quality = 80, int maxWidth = 1024, int maxHeight = 1024);

    /// <summary>
    /// Save image to app's local storage
    /// </summary>
    /// <param name="imagePath">Source image path</param>
    /// <param name="fileName">Target filename</param>
    /// <returns>Path to the saved image</returns>
    Task<string> SaveImageToAppStorageAsync(string imagePath, string fileName);

    /// <summary>
    /// Delete an image from app storage
    /// </summary>
    /// <param name="imagePath">Path to the image to delete</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteImageAsync(string imagePath);

    /// <summary>
    /// Get image dimensions
    /// </summary>
    /// <param name="imagePath">Path to the image</param>
    /// <returns>Tuple of width and height</returns>
    Task<(int width, int height)> GetImageDimensionsAsync(string imagePath);

    /// <summary>
    /// Check if camera is available on the device
    /// </summary>
    /// <returns>True if camera is available</returns>
    Task<bool> IsCameraAvailableAsync();

    /// <summary>
    /// Check if photo gallery is available on the device
    /// </summary>
    /// <returns>True if gallery is available</returns>
    Task<bool> IsGalleryAvailableAsync();

    /// <summary>
    /// Request camera permission
    /// </summary>
    /// <returns>True if permission granted</returns>
    Task<bool> RequestCameraPermissionAsync();

    /// <summary>
    /// Request photo gallery permission
    /// </summary>
    /// <returns>True if permission granted</returns>
    Task<bool> RequestGalleryPermissionAsync();
}