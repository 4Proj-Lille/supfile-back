using SupFile.Back.Core.Entities.Auth;
using SupFile.Back.Core.Errors;

namespace SupFile.Back.Data.Repositories;

public class FolderRepository : BaseRepository<Folder, int, SupFileContext>, IFolderRepository
{
    public FolderRepository(ILogger<FolderRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user)
    {
        var softDeletedFoldersResult = await GetSoftDeleted<Folder>(user);
        if (softDeletedFoldersResult.IsFailed) return softDeletedFoldersResult
            .ToResult<int>();
        
        if (softDeletedFoldersResult.Value.Count == 0) return Result.Fail(BinErrors.NoFolderFound());
        
        return await DeleteAllAsync(x => x.OwnerId == user.Id && !x.IsActive);
    }

    public async Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? id, string filter, string orderBy, bool shared, int? limit = null)
    {
        IQueryable<Folder> q = Query().Where(x =>
            x.ParentId == id && x.IsActive &&
            (shared || x.OwnerId == user.Id)
        ).OrderBy(x => x.Name);

        if (limit.HasValue)
            q = q.Take(limit.Value);

        return Result.Ok(await q.FindListAsync<TMapped>(filter, orderBy: orderBy));
    }

    public async Task<Result<List<TMapped>>> GetPath<TMapped>(ApplicationUser user, int? id)
    {
        var path = new List<TMapped>();
        var currentId = id;

        while (currentId != null)
        {
            var info = await Query()
                .Where(x => x.Id == currentId && x.OwnerId == user.Id && x.IsActive)
                .Select(x => new { x.Id, x.ParentId })
                .FirstOrDefaultAsync();

            if (info == null)
                return Result.Fail(EntityErrors.NotFound<TMapped>());

            var mapped = await Query()
                .Where(x => x.Id == info.Id)
                .ProjectToType<TMapped>()
                .FirstOrDefaultAsync();

            path.Add(mapped!);
            currentId = info.ParentId;
        }

        path.Reverse();
        return Result.Ok(path);
    }

    public async Task<Result<List<Folder>>> GetAncestorsAsync(int folderId)
    {
        var ancestors = new List<Folder>();
        int? currentId = folderId;

        while (currentId != null)
        {
            var folder = await Query()
                .Where(x => x.Id == currentId && x.IsActive)
                .FirstOrDefaultAsync();

            if (folder == null) break;

            ancestors.Add(folder);
            currentId = folder.ParentId;
        }

        ancestors.Reverse();
        return Result.Ok(ancestors);
    }

    public async Task<Result<int>> SoftDeleteChildrensAsync(int folderId)
    {
        var descendantIdsResult = await GetAllDescendantIdsAsync(folderId);
        if (descendantIdsResult.IsFailed) return descendantIdsResult.ToResult();
        var descendantIds = descendantIdsResult.Value;

        if (descendantIds.Count == 0) return Result.Ok(0);

        var affected = await Query()
            .Where(x => descendantIds.Contains(x.Id))
            .ExecuteUpdateAsync(x => x.SetProperty(f => f.IsActive, false));

        return Result.Ok(affected);
    }

    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && !x.IsActive
        );

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }

    public async Task<Result<List<int>>> GetAllDescendantIdsAsync(int folderId, bool onlyActive = true)
    {
        var result = new List<int>();
        var queue = new Queue<int>();
        queue.Enqueue(folderId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();

            var childIds = await Query()
                .Where(x => x.ParentId == currentId && (!onlyActive || x.IsActive))
                .Select(x => x.Id)
                .ToListAsync();

            foreach (var childId in childIds)
            {
                result.Add(childId);
                queue.Enqueue(childId);
            }
        }

        return Result.Ok(result);
    }
    
    public async Task<Result<long>> GetFolderSizeRecursive(int? folderId)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        var medias = await ctx.Medias
            .Where(m => m.FolderId == folderId && m.IsActive)
            .ToListAsync();

        long size = medias.Sum(m => (long)m.Size);

        var subFolders = await ctx.Folders
            .Where(f => f.ParentId == folderId && f.IsActive)
            .ToListAsync();

        foreach (var folder in subFolders)
        {
            var result = await GetFolderSizeRecursive(folder.Id);

            if (!result.IsSuccess)
                return result;

            size += result.Value;        }

        return Result.Ok(size);
    }
    
    public async Task<Result<int>> RestoreByIdsAsync(List<int> folderIds)
    {
        var affected = await Query()
            .Where(x => folderIds.Contains(x.Id) && !x.IsActive)
            .ExecuteUpdateAsync(x => x.SetProperty(f => f.IsActive, true));

        return affected;
    }

    public async Task<Result<List<Folder>>> GetSubfoldersPublicAsync(int parentId)
    {
        var folders = await Query()
            .Where(x => x.ParentId == parentId && x.IsActive)
            .ToListAsync();

        return Result.Ok(folders);
    }
}
