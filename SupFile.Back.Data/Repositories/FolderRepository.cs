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
        
        if (softDeletedFoldersResult.Value.Count == 0) return Result.Ok(0);
        
        return await DeleteAllAsync(x => x.OwnerId == user.Id && !x.IsActive);
    }

    public async Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? id, string filter, string orderBy)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.ParentId == id && x.IsActive
        ).OrderBy(x => x.Name);

        return Result.Ok(await q.FindListAsync<TMapped>(filter, orderBy: orderBy));
    }

    public async Task<Result<List<Folder>>> GetPath(ApplicationUser user, int? id)
    {
        var path = new List<Folder>();
        var currentId = id;

        while (currentId != null)
        {
            var folderResult =
                await Query().Where(x => x.Id == currentId && x.OwnerId == user.Id && x.IsActive).FirstOrDefaultAsync();
            if (folderResult == null)
            {
                return Result.Fail(EntityErrors.NotFound<Folder>());
            }

            path.Add(folderResult);
            currentId = folderResult.ParentId;
        }

        path.Reverse();
        return Result.Ok(path);
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
}
