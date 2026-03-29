namespace SupFile.Back.Core.Interfaces.Repositories.Base;

public interface IBaseRepository<T, TId> where T : IEntity<T, TId>
{
    Task<Result<T>> AddAsync(T entity, CancellationToken ct = default);

    Task<Result<T>> UpdateAsync(TId id, T entity, CancellationToken ct = default);

    Task<Result> DeleteAsync(T entity, CancellationToken ct = default);
    Task<Result> DeleteAsync(TId id, CancellationToken ct = default);
    Task<Result<int>> DeleteAllAsync(Expression<Func<T, bool>> filterExpression, CancellationToken ct = default);

    Task<Result<List<TMapped>>> FindListAsync<TMapped>(string filter, string? orderBy = null, CancellationToken ct = default);

    // Task<List<TMapped>> FindListAsync<TMapped, TOrderBy>(Expression<Func<T, bool>> filterExpression, Expression<Func<T, TOrderBy>> orderByExpression, bool descending = false, CancellationToken ct = default);
    Task<Result<List<TMapped>>> FindListAsync<TMapped, TOrderBy>(
        Expression<Func<T, bool>> filterExpression,
        Expression<Func<T, TOrderBy>> orderByExpression,
        bool descending = false,
        CancellationToken ct = default);

    Task<Result<TMapped>> FindOneAsync<TMapped>(string filter, CancellationToken ct = default);

    Task<Result<TMapped>> FindOneAsync<TMapped>(Expression<Func<TMapped, bool>> filterExpression,
        CancellationToken ct = default);

    Task<Result<TMapped>> GetByIdAsync<TMapped>(TId id, CancellationToken ct = default);

    Task<Result<List<TMapped>>> GetAllAsync<TMapped>(CancellationToken ct = default);
}
