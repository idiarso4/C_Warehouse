using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repository properties
    IRepository<Product> Products { get; }
    IRepository<Category> Categories { get; }
    IRepository<Location> Locations { get; }
    IRepository<StockMovement> StockMovements { get; }
    IRepository<ProductLocation> ProductLocations { get; }
    IRepository<User> Users { get; }
    IRepository<Supplier> Suppliers { get; }

    // Transaction methods
    int SaveChanges();
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    // Transaction management
    void BeginTransaction();
    Task BeginTransactionAsync();
    void CommitTransaction();
    Task CommitTransactionAsync();
    void RollbackTransaction();
    Task RollbackTransactionAsync();

    // Bulk operations
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken, params object[] parameters);

    // Database operations
    Task EnsureCreatedAsync();
    Task EnsureDeletedAsync();
    Task MigrateAsync();

    // Generic repository access
    IRepository<T> Repository<T>() where T : BaseEntity;
}