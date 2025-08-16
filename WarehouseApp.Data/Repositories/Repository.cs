using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WarehouseApp.Core.Models;
using WarehouseApp.Data.Context;
using WarehouseApp.Data.Interfaces;

namespace WarehouseApp.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly WarehouseDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(WarehouseDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Synchronous methods
    public virtual T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public virtual T? GetById(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query.FirstOrDefault(e => e.Id == id);
    }

    public virtual IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public virtual IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query.ToList();
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate).ToList();
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query.Where(predicate).ToList();
    }

    public virtual T? FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.FirstOrDefault(predicate);
    }

    public virtual T? FirstOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query.FirstOrDefault(predicate);
    }

    // Asynchronous methods
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(predicate);
    }

    // Pagination
    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _dbSet.CountAsync();
        var items = await _dbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, bool ascending = true)
    {
        var totalCount = await _dbSet.CountAsync();

        IQueryable<T> query = _dbSet;
        query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate, Expression<Func<T, TKey>> orderBy, bool ascending = true)
    {
        IQueryable<T> query = _dbSet;

        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync();

        query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    // Count methods
    public virtual int Count()
    {
        return _dbSet.Count();
    }

    public virtual int Count(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Count(predicate);
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    // Existence check
    public virtual bool Any(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Any(predicate);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    // CRUD operations
    public virtual void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    // Soft delete
    public virtual void SoftDelete(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        Update(entity);
    }

    public virtual void SoftDeleteRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        UpdateRange(entities);
    }

    // Include deleted entities
    public virtual IEnumerable<T> GetAllIncludingDeleted()
    {
        return _dbSet.IgnoreQueryFilters().ToList();
    }

    public virtual async Task<IEnumerable<T>> GetAllIncludingDeletedAsync()
    {
        return await _dbSet.IgnoreQueryFilters().ToListAsync();
    }

    public virtual IEnumerable<T> FindIncludingDeleted(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.IgnoreQueryFilters().Where(predicate).ToList();
    }

    public virtual async Task<IEnumerable<T>> FindIncludingDeletedAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.IgnoreQueryFilters().Where(predicate).ToListAsync();
    }

    // Raw SQL
    public virtual IEnumerable<T> FromSqlRaw(string sql, params object[] parameters)
    {
        return _dbSet.FromSqlRaw(sql, parameters).ToList();
    }

    public virtual async Task<IEnumerable<T>> FromSqlRawAsync(string sql, params object[] parameters)
    {
        return await _dbSet.FromSqlRaw(sql, parameters).ToListAsync();
    }

    // Queryable for advanced scenarios
    public virtual IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }
}