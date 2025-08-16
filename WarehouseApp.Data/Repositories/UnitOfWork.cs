using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WarehouseApp.Core.Models;
using WarehouseApp.Data.Context;
using WarehouseApp.Data.Interfaces;

namespace WarehouseApp.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly WarehouseDbContext _context;
    private readonly Dictionary<Type, object> _repositories;
    private IDbContextTransaction? _transaction;

    // Repository properties
    private IRepository<Product>? _products;
    private IRepository<Category>? _categories;
    private IRepository<Location>? _locations;
    private IRepository<StockMovement>? _stockMovements;
    private IRepository<ProductLocation>? _productLocations;
    private IRepository<User>? _users;
    private IRepository<Supplier>? _suppliers;

    public UnitOfWork(WarehouseDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    // Repository properties with lazy initialization
    public IRepository<Product> Products => _products ??= new Repository<Product>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<Location> Locations => _locations ??= new Repository<Location>(_context);
    public IRepository<StockMovement> StockMovements => _stockMovements ??= new Repository<StockMovement>(_context);
    public IRepository<ProductLocation> ProductLocations => _productLocations ??= new Repository<ProductLocation>(_context);
    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Supplier> Suppliers => _suppliers ??= new Repository<Supplier>(_context);

    // Generic repository access
    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);

        if (_repositories.ContainsKey(type))
        {
            return (IRepository<T>)_repositories[type];
        }

        var repository = new Repository<T>(_context);
        _repositories.Add(type, repository);
        return repository;
    }

    // Transaction methods
    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // Transaction management
    public void BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public void CommitTransaction()
    {
        try
        {
            _transaction?.Commit();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _transaction?.Rollback();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    // Bulk operations
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    public async Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, cancellationToken, parameters);
    }

    // Database operations
    public async Task EnsureCreatedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task EnsureDeletedAsync()
    {
        await _context.Database.EnsureDeletedAsync();
    }

    public async Task MigrateAsync()
    {
        await _context.Database.MigrateAsync();
    }

    // Dispose pattern
    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}