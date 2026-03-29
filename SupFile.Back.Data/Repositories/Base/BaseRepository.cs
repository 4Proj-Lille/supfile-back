using SupFile.Back.Core.Errors;
using SupFile.Back.Core.Interfaces.Entities;

namespace SupFile.Back.Data.Repositories.Base;

public abstract class BaseRepository<T, TId, TDbContext> : IBaseRepository<T, TId>
    where T : class, IEntity<T, TId>
    where TDbContext : DbContext
{
    protected BaseRepository(ILogger logger, IDbContextFactory<TDbContext> contextFactory)
    {
        Logger = logger;
        ContextFactory = contextFactory;
        Context = contextFactory.CreateDbContext();
    }

    private TDbContext Context { get; }
    protected IDbContextFactory<TDbContext> ContextFactory { get; }
    protected ILogger Logger { get; }

    public virtual async Task<Result<T>> AddAsync(T entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var addedEntityEntry = await Context.AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
        var result = addedEntityEntry.Entity;

        return result;
    }

    public virtual async Task<Result<T>> UpdateAsync(TId id, T entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var myContext = await ContextFactory.CreateDbContextAsync(ct);
        var updatedEntityEntry = myContext.Update(entity);
        await myContext.SaveChangesAsync(ct);

        return updatedEntityEntry.Entity;
    }

    #region GetByIdAsync

    public async Task<Result<TMapped>> GetByIdAsync<TMapped>(TId id, CancellationToken ct = default)
    {
        var entity = await Query().FindByIdAsync<TMapped, TId>(id, ct);
        if (entity is null) return Result.Fail(EntityErrors.NotFound<T>());

        return Result.Ok(entity);
    }

    #endregion GetByIdAsync

    #region GetAllAsync

    public async Task<Result<List<TMapped>>> GetAllAsync<TMapped>(CancellationToken ct = default)
    {
        return await Query().FindListAsync<TMapped>("", null, ct);
    }

    #endregion GetAllAsync

    protected IQueryable<T> Query()
    {
        return Context.Set<T>();
    }

    #region DeleteAsync

    public virtual async Task<Result> DeleteAsync(T entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var myContext = await ContextFactory.CreateDbContextAsync(ct);
        myContext.Remove(entity);
        var numberOfChanges = await myContext.SaveChangesAsync(ct);
        if (numberOfChanges == 0) return Result.Fail(EntityErrors.NotFound<T>());

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(TId id, CancellationToken ct = default)
    {
        await using var myContext = await ContextFactory.CreateDbContextAsync(ct);
        var entity = await myContext.FindAsync<T>([id], ct);
        if (entity == null)
        {
            return Result.Fail(EntityErrors.NotFound<T>());
        }

        myContext.Remove(entity);
        var numberOfChanges = await myContext.SaveChangesAsync(ct);
        if (numberOfChanges == 0) return Result.Fail(EntityErrors.NotFound<T>());

        return Result.Ok();
    }

    #endregion DeleteAsync


    #region DeleteAllAsync

    public async Task<Result<int>> DeleteAllAsync(Expression<Func<T, bool>> filterExpression,
        CancellationToken ct = default)
    {
        var deletedCount = await Query().Where(filterExpression).ExecuteDeleteAsync(ct);
        if (deletedCount == 0)
        {
            return Result.Fail(EntityErrors.NotFound<T>());
        }

        return Result.Ok(deletedCount);
    }

    #endregion DeleteAllAsync

    #region FindListAsync

    public async Task<Result<List<TMapped>>> FindListAsync<TMapped>(string filter, string? orderBy = null,
        CancellationToken ct = default)
    {
        return await Query().FindListAsync<TMapped>(filter, orderBy, ct);
    }

    public async Task<Result<List<TMapped>>> FindListAsync<TMapped, TOrderBy>(
        Expression<Func<T, bool>> filterExpression,
        Expression<Func<T, TOrderBy>> orderByExpression, bool descending = false, CancellationToken ct = default)
    {
        var q = Query().Where(filterExpression);
        var resultList = await q.FindListAsync<T, TMapped, TOrderBy>(x => true, orderByExpression, descending, ct);
        if (resultList.Count == 0) return Result.Fail(EntityErrors.NotFound<T>());
        return Result.Ok(resultList);
    }

    #endregion FindListAsync

    #region FindOneAsync

    public async Task<Result<TMapped>> FindOneAsync<TMapped>(string filter, CancellationToken ct = default)
    {
        var entity = await Query().FindOneAsync<TMapped>(filter, ct);
        if (entity is null) return Result.Fail(EntityErrors.NotFound<T>());
        return Result.Ok(entity);
    }

    public async Task<Result<TMapped>> FindOneAsync<TMapped>(Expression<Func<TMapped, bool>> filterExpression,
        CancellationToken ct = default)
    {
        var entity = await Query().FindOneAsync(filterExpression, ct);
        if (entity is null) return Result.Fail(EntityErrors.NotFound<T>());
        return Result.Ok(entity);
    }

    #endregion
}
