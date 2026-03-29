namespace SupFile.Back.Business.Services.Base;

public class BaseService<T, TId, TRepository> : IBaseService<T, TId>
    where T : IEntity<T, TId>, new()
    where TRepository : IBaseRepository<T, TId>
{
    protected BaseService(ILogger logger, TRepository repository)
    {
        Logger = logger;
        Repository = repository;
    }

    protected ILogger Logger { get; }
    protected TRepository Repository { get; }

    public async Task<Result<T>> AddAsync(T entity, CancellationToken ct = default)
    {
        return await Repository.AddAsync(entity, ct);
    }

    public async Task<Result<T>> UpdateAsync(TId id, T entity, CancellationToken ct = default)
    {
        return await Repository.UpdateAsync(id, entity, ct);
    }

    public async Task<Result> DeleteAsync(T entity, CancellationToken ct = default)
    {
        return await Repository.DeleteAsync(entity, ct);
    }

    public async Task<Result> DeleteAsync(TId id, CancellationToken ct = default)
    {
        return await Repository.DeleteAsync(id, ct);
    }

    public async Task<Result<TMapped>> GetByIdAsync<TMapped>(TId id, CancellationToken ct = default)
    {
        var entityResult = await Repository.GetByIdAsync<TMapped>(id, ct);
        if (entityResult.IsFailed) return entityResult;

        return Result.Ok(entityResult.Value);
    }

    public async Task<Result<List<TMapped>>> GetAllAsync<TMapped>(CancellationToken ct = default)
    {
        return await Repository.GetAllAsync<TMapped>(ct);
    }

    public async Task<Result<TMapped>> GetOneAsync<TMapped>(Expression<Func<TMapped, bool>> filterExpression,
        CancellationToken ct = default)
    {
        return await Repository.FindOneAsync(filterExpression, ct);
    }


    public async Task<Result<List<TMapped>>> FindListAsync<TMapped, TOrderBy>(
        Expression<Func<T, bool>> filterExpression,
        Expression<Func<T, TOrderBy>> orderByExpression,
        bool descending = false,
        CancellationToken ct = default)
    {
        return await Repository.FindListAsync<TMapped, TOrderBy>(filterExpression, orderByExpression, descending, ct);
    }
}
