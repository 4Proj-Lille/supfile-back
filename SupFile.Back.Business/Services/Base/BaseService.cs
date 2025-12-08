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
        try
        {
            return Result.Ok(await Repository.AddAsync(entity, ct));
        }
        catch (Exception e)
        {
            return Result.Fail(new CustomError(HttpStatusCode.InternalServerError, e.Message));
        }
    }

    public async Task<Result<T>> UpdateAsync(TId id, T entity, CancellationToken ct = default)
    {
        try
        {
            return Result.Ok(await Repository.UpdateAsync(id, entity, ct));
        }
        catch (Exception e)
        {
            return Result.Fail(new CustomError(HttpStatusCode.InternalServerError, e.Message));
        }
    }

    public async Task<Result<bool>> DeleteAsync(T entity, CancellationToken ct = default)
    {
        try
        {
            return Result.Ok(await Repository.DeleteAsync(entity, ct));
        }
        catch (Exception e)
        {
            return Result.Fail(new CustomError(HttpStatusCode.InternalServerError, e.Message));
        }
    }

    public async Task<Result<bool>> DeleteAsync<TMapped>(TId id, CancellationToken ct = default)
    {
        try
        {
            return Result.Ok(await Repository.DeleteAsync(id, ct));
        }
        catch (Exception e)
        {
            return Result.Fail(new CustomError(HttpStatusCode.InternalServerError, e.Message));
        }
    }

    public async Task<Result<TMapped>> GetByIdAsync<TMapped>(TId id, CancellationToken ct = default)
    {
        try
        {
            var entityResult = await Repository.GetByIdAsync<TMapped>(id, ct);
            if (entityResult.IsFailed || entityResult.Value == null)
            {
                return Result.Fail(entityResult.Errors);
            }

            return Result.Ok(entityResult.Value);
        }
        catch (Exception e)
        {
            return Result.Fail(new CustomError(HttpStatusCode.InternalServerError, e.Message));
        }
    }

    public async Task<Result<List<TMapped>>> GetAllAsync<TMapped>(CancellationToken ct = default)
    {
        try
        {
            return Result.Ok(await Repository.GetAllAsync<TMapped>(ct));
        }
        catch (Exception e)
        {
            return Result.Fail(new CustomError(HttpStatusCode.InternalServerError, e.Message));
        }
    }

    public async Task<Result<TMapped>> GetOneAsync<TMapped>(Expression<Func<TMapped, bool>> filterExpression,
        CancellationToken ct = default)
    {
        try
        {
            var item = await Repository.FindOneAsync(filterExpression, ct);

            if (item == null)
            {
                return Result.Fail(new NotFoundError($"{typeof(T).Name} not found"));
            }

            return Result.Ok(item);
        }
        catch (Exception e)
        {
            return Result.Fail(new CustomError(HttpStatusCode.InternalServerError, e.Message));
        }
    }


    public async Task<Result<List<TMapped>>> FindListAsync<TMapped, TOrderBy>(
        Expression<Func<T, bool>> filterExpression,
        Expression<Func<T, TOrderBy>> orderByExpression,
        bool descending = false,
        CancellationToken ct = default)
    {
        try
        {
            return await Repository.FindListAsync<TMapped, TOrderBy>(filterExpression, orderByExpression, descending,
                ct);
        }
        catch (Exception e)
        {
            return Result.Fail(new CustomError(HttpStatusCode.InternalServerError, e.Message));
        }
    }
}
