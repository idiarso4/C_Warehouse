namespace WarehouseApp.MAUI.Services;

/// <summary>
/// Service locator helper for accessing DI services from anywhere in the app
/// </summary>
public static class ServiceHelper
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Initialize the service provider
    /// </summary>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Get a service of type T
    /// </summary>
    public static T GetService<T>() where T : class
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ServiceHelper not initialized. Call Initialize() first.");

        var service = _serviceProvider.GetService<T>();
        if (service == null)
            throw new InvalidOperationException($"Service of type {typeof(T).Name} not registered.");

        return service;
    }

    /// <summary>
    /// Get a required service of type T
    /// </summary>
    public static T GetRequiredService<T>() where T : class
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ServiceHelper not initialized. Call Initialize() first.");

        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Try to get a service of type T
    /// </summary>
    public static T? TryGetService<T>() where T : class
    {
        if (_serviceProvider == null)
            return null;

        return _serviceProvider.GetService<T>();
    }

    /// <summary>
    /// Get all services of type T
    /// </summary>
    public static IEnumerable<T> GetServices<T>() where T : class
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ServiceHelper not initialized. Call Initialize() first.");

        return _serviceProvider.GetServices<T>();
    }

    /// <summary>
    /// Create a scope for scoped services
    /// </summary>
    public static IServiceScope CreateScope()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("ServiceHelper not initialized. Call Initialize() first.");

        return _serviceProvider.CreateScope();
    }

    /// <summary>
    /// Check if service provider is initialized
    /// </summary>
    public static bool IsInitialized => _serviceProvider != null;
}