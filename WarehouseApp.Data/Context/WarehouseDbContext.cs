using Microsoft.EntityFrameworkCore;
using WarehouseApp.Core.Models;
using WarehouseApp.Data.Configurations;

namespace WarehouseApp.Data.Context;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<ProductLocation> ProductLocations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new LocationConfiguration());
        modelBuilder.ApplyConfiguration(new StockMovementConfiguration());
        modelBuilder.ApplyConfiguration(new ProductLocationConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());

        // Global query filters for soft delete
        modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Location>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StockMovement>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ProductLocation>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Supplier>().HasQueryFilter(e => !e.IsDeleted);

        // Seed data
        SeedData(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Default SQLite connection for development
            optionsBuilder.UseSqlite("Data Source=warehouse.db");
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps before saving
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        // Update timestamps before saving
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic products", Color = "#007ACC", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Category { Id = 2, Name = "Clothing", Description = "Clothing and apparel", Color = "#FF6B6B", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Category { Id = 3, Name = "Books", Description = "Books and publications", Color = "#4ECDC4", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Category { Id = 4, Name = "Home & Garden", Description = "Home and garden products", Color = "#45B7D1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        // Seed default locations
        modelBuilder.Entity<Location>().HasData(
            new Location { Id = 1, Code = "A1-01-A-01", Name = "Zone A - Aisle 1 - Shelf A - Position 1", Zone = "A", Aisle = "1", Shelf = "A", Position = "1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Location { Id = 2, Code = "A1-01-A-02", Name = "Zone A - Aisle 1 - Shelf A - Position 2", Zone = "A", Aisle = "1", Shelf = "A", Position = "2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Location { Id = 3, Code = "B1-01-A-01", Name = "Zone B - Aisle 1 - Shelf A - Position 1", Zone = "B", Aisle = "1", Shelf = "A", Position = "1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Location { Id = 4, Code = "B1-01-A-02", Name = "Zone B - Aisle 1 - Shelf A - Position 2", Zone = "B", Aisle = "1", Shelf = "A", Position = "2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        // Seed default admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@warehouse.com",
                FirstName = "System",
                LastName = "Administrator",
                PasswordHash = "$2a$11$rQiU9k7Z8k7Z8k7Z8k7Z8O", // This should be properly hashed
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}