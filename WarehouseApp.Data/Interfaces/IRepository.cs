using System.Linq.Expressions;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    // Synchronous methods
    T? GetById(int id);
    T? GetById(int id, params Expression<Func<T, object>>[] includes);
    IEnumerable<T> GetAll();
    IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);
    T? FirstOrDefault(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    // Asynchronous methods
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    // Pagination
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, TKey>> orderBy, bool ascending = true);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync<TKey>(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate, Expression<Func<T, TKey>> orderBy, bool ascending = true);

    // Count methods
    int Count();
    int Count(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    // Existence check
    bool Any(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    // CRUD operations
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    // Soft delete
    void SoftDelete(T entity);
    void SoftDeleteRange(IEnumerable<T> entities);

    // Include deleted entities
    IEnumerable<T> GetAllIncludingDeleted();
    Task<IEnumerable<T>> GetAllIncludingDeletedAsync();
    IEnumerable<T> FindIncludingDeleted(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindIncludingDeletedAsync(Expression<Func<T, bool>> predicate);

    // Raw SQL
    IEnumerable<T> FromSqlRaw(string sql, params object[] parameters);
    Task<IEnumerable<T>> FromSqlRawAsync(string sql, params object[] parameters);

    // Queryable for advanced scenarios
    IQueryable<T> GetQueryable();
}