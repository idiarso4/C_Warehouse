using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseApp.Data.Context;
using WarehouseApp.Data.Interfaces;

namespace WarehouseApp.MAUI.Services;

public class DatabaseInitializationService : IDatabaseInitializationService
{
    private readonly WarehouseDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DatabaseInitializationService> _logger;

    public DatabaseInitializationService(
        WarehouseDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<DatabaseInitializationService> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Initializing database...");

            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Apply any pending migrations
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await MigrateAsync();
            }

            // Seed initial data if database is empty
            if (!await IsDatabaseInitializedAsync())
            {
                await SeedDataAsync();
            }

            _logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database initialization");
            throw;
        }
    }

    public async Task SeedDataAsync()
    {
        try
        {
            _logger.LogInformation("Seeding database with initial data...");

            // Check if data already exists
            if (await _unitOfWork.Categories.AnyAsync(c => true))
            {
                _logger.LogInformation("Database already contains data, skipping seed");
                return;
            }

            // The seed data is already configured in DbContext.SeedData()
            // This will be automatically applied when the database is created

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database seeding");
            throw;
        }
    }

    public async Task MigrateAsync()
    {
        try
        {
            _logger.LogInformation("Applying database migrations...");

            await _context.Database.MigrateAsync();

            _logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database migration");
            throw;
        }
    }

    public async Task ResetDatabaseAsync()
    {
        try
        {
            _logger.LogWarning("Resetting database - all data will be lost!");

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
            await SeedDataAsync();

            _logger.LogInformation("Database reset completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database reset");
            throw;
        }
    }

    public async Task<bool> IsDatabaseInitializedAsync()
    {
        try
        {
            // Check if essential tables have data
            var hasCategories = await _unitOfWork.Categories.AnyAsync(c => true);
            var hasLocations = await _unitOfWork.Locations.AnyAsync(l => true);
            var hasUsers = await _unitOfWork.Users.AnyAsync(u => true);

            return hasCategories && hasLocations && hasUsers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database initialization status");
            return false;
        }
    }
}