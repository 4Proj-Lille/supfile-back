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

    protected TDbContext Context { get; }
    protected IDbContextFactory<TDbContext> ContextFactory { get; }
    protected ILogger Logger { get; }

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            var addedEntityEntry = await Context.AddAsync(entity, ct);
            await Context.SaveChangesAsync(ct);
            var result = addedEntityEntry.Entity;

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public virtual async Task<T> UpdateAsync(TId id, T entity, CancellationToken ct = default)
    {
        // throw new NotImplementedException();
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            await using var myContext = ContextFactory.CreateDbContext();
            var updatedEntityEntry = myContext.Update(entity);
            await myContext.SaveChangesAsync(ct);

            return updatedEntityEntry.Entity;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #region GetByIdAsync

    public async Task<Result<TMapped>> GetByIdAsync<TMapped>(TId id, CancellationToken ct = default)
    {
        var result = await Query().FindByIdAsync<TMapped, TId>(id, ct);
        if (result == null)
        {
            return Result.Fail(new NotFoundError($"{typeof(T).Name} {id} not found"));
        }

        return Result.Ok(result);
    }

    #endregion GetByIdAsync

    #region GetAllAsync

    public async Task<List<TMapped>> GetAllAsync<TMapped>(CancellationToken ct = default)
    {
        return await Query().FindListAsync<TMapped>("", null, ct);
    }

    #endregion GetAllAsync

    protected IQueryable<T> Query()
    {
        return Context.Set<T>();
    }

    #region DeleteAsync

    public virtual async Task<bool> DeleteAsync(T entity, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            await using var myContext = await ContextFactory.CreateDbContextAsync(ct);
            myContext.Remove(entity);
            var numberOfChanges = await myContext.SaveChangesAsync(ct);
            return numberOfChanges > 0;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteAsync(TId id, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        try
        {
            await using var myContext = await ContextFactory.CreateDbContextAsync(ct);
            var entity = await myContext.FindAsync<T>(id);
            if (entity == null)
            {
                return false;
            }

            myContext.Remove(entity);
            var numberOfChanges = await myContext.SaveChangesAsync(ct);
            return numberOfChanges > 0;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion DeleteAsync

    #region FindListAsync

    public async Task<List<TMapped>> FindListAsync<TMapped>(string filter, string? orderBy = null,
        CancellationToken ct = default)
    {
        return await Query().FindListAsync<TMapped>(filter, orderBy, ct);
    }

    public async Task<Result<List<TMapped>>> FindListAsync<TMapped, TOrderBy>(
        Expression<Func<T, bool>> filterExpression,
        Expression<Func<T, TOrderBy>> orderByExpression, bool descending = false, CancellationToken ct = default)
    {
        try
        {
            var q = Query().Where(filterExpression);
            var resultList = await q.FindListAsync<T, TMapped, TOrderBy>(x => true, orderByExpression, descending, ct);

            return Result.Ok(resultList);
        }
        catch (Exception)
        {
            return Result.Fail(new NotFoundError($"{typeof(T).Name} {filterExpression} not found"));
        }
    }

    #endregion FindListAsync

    #region FindOneAsync

    public async Task<TMapped?> FindOneAsync<TMapped>(string filter, CancellationToken ct = default)
    {
        return await Query().FindOneAsync<TMapped>(filter, ct);
    }

    public async Task<TMapped?> FindOneAsync<TMapped>(Expression<Func<TMapped, bool>> filterExpression,
        CancellationToken ct = default)
    {
        return await Query().FindOneAsync(filterExpression, ct);
    }

    #endregion
}
