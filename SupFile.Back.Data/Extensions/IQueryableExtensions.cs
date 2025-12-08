namespace SupFile.Back.Data.Extensions;

public static class IQueryableExtensions
{
    #region FindByIdAsync

    public static async Task<T?> FindByIdAsync<T, TId>(this IQueryable query, TId id, CancellationToken ct = default)
    {
        var projectedQuery = query.ProjectToType<T>();
        projectedQuery = projectedQuery.ApplyFiltering($"Id = {id}");
        var result = await projectedQuery.SingleOrDefaultAsync(ct);

        return result;
    }

    #endregion FindByIdAsync

    #region FindOneAsync

    public static async Task<T?> FindOneAsync<T>(this IQueryable query, string filter, CancellationToken ct = default)
    {
        var projectedQuery = query.ProjectToType<T>();
        projectedQuery = projectedQuery.ApplyFiltering(filter);
        var result = await projectedQuery.SingleOrDefaultAsync(ct);

        return result;
    }

    public static async Task<T?> FindOneAsync<T>(this IQueryable query, Expression<Func<T, bool>> filterExpression,
        CancellationToken ct = default)
    {
        var projectedQuery = query.ProjectToType<T>();
        projectedQuery = projectedQuery.Where(filterExpression);
        var result = await projectedQuery.SingleOrDefaultAsync(ct);

        return result;
    }

    #endregion FindOneAsync

    #region FindListAsync

    public static async Task<List<T>> FindListAsync<T>(this IQueryable query, string filter, string? orderBy = null,
        CancellationToken ct = default)
    {
        var projectedQuery = query.ProjectToType<T>();
        projectedQuery = projectedQuery.ApplyFiltering(filter);

        if (orderBy != null)
        {
            projectedQuery = projectedQuery.ApplyOrdering(orderBy);
        }

        var result = await projectedQuery.ToListAsync(ct);

        return result;
    }

    public static async Task<List<TMapped>> FindListAsync<TSource, TMapped, TOrderBy>(this IQueryable<TSource> query,
        Expression<Func<TSource, bool>> filterExpression, Expression<Func<TSource, TOrderBy>> orderByExpression,
        bool descending = false, CancellationToken ct = default)
    {
        query = query.Where(filterExpression);
        query = descending ? query.OrderByDescending(orderByExpression) : query.OrderBy(orderByExpression);

        var projectedQuery = query.ProjectToType<TMapped>();

        return await projectedQuery.ToListAsync(ct);
    }

    #endregion FindListAsync
}
