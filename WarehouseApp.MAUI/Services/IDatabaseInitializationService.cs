namespace WarehouseApp.MAUI.Services;

public interface IDatabaseInitializationService
{
    Task InitializeAsync();
    Task SeedDataAsync();
    Task MigrateAsync();
    Task ResetDatabaseAsync();
    Task<bool> IsDatabaseInitializedAsync();
}