namespace SupFile.Back.Core.Interfaces.Repositories.Base;

public interface IBaseRepository<T, TId> where T : IEntity<T, TId>
{
    Task<T> AddAsync(T entity, CancellationToken ct = default);

    Task<T> UpdateAsync(TId id, T entity, CancellationToken ct = default);

    Task<bool> DeleteAsync(T entity, CancellationToken ct = default);
    Task<bool> DeleteAsync(TId id, CancellationToken ct = default);

    Task<List<TMapped>> FindListAsync<TMapped>(string filter, string? orderBy = null, CancellationToken ct = default);

    // Task<List<TMapped>> FindListAsync<TMapped, TOrderBy>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, TOrderBy>> orderByExpression, bool descending = false, CancellationToken ct = default);
    Task<Result<List<TMapped>>> FindListAsync<TMapped, TOrderBy>(
        Expression<Func<T, bool>> filterExpression,
        Expression<Func<T, TOrderBy>> orderByExpression,
        bool descending = false,
        CancellationToken ct = default);

    Task<TMapped?> FindOneAsync<TMapped>(string filter, CancellationToken ct = default);

    Task<TMapped?> FindOneAsync<TMapped>(Expression<Func<TMapped, bool>> filterExpression,
        CancellationToken ct = default);

    Task<Result<TMapped>> GetByIdAsync<TMapped>(TId id, CancellationToken ct = default);

    Task<List<TMapped>> GetAllAsync<TMapped>(CancellationToken ct = default);
}
