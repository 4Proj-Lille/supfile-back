using SupFile.Back.Core.Entities.Auth;
using SupFile.Back.Core.Errors;

namespace SupFile.Back.Data.Repositories;

public class MediaRepository : BaseRepository<Media, int, SupFileContext>, IMediaRepository
{
    public MediaRepository(ILogger<MediaRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? folderId,
        string filter, string orderBy, bool shared = false)
    {
        var q = Query().Where(x =>
            x.FolderId == folderId && x.IsActive && x.UniqueId != user.ProfilePictureId &&
            (shared || x.OwnerId == user.Id)
        );

        var result = await q.FindListAsync<TMapped>(filter, orderBy: orderBy);
        return Result.Ok(result);
    }

    public async Task<Result<Dictionary<string, int>>> GetTotalStorageSize(ApplicationUser user)
    {
        return Result.Ok(
            new Dictionary<string, int>
            {
                ["Global"] = await Query()
                    .Where(x => x.OwnerId == user.Id && x.UniqueId != user.ProfilePictureId)
                    .SumAsync(x => x.Size)
            }
        );
    }

    public async Task<Result<Dictionary<string, int>>> GetStorageSizeByExtension(ApplicationUser user)
    {
        var storageSizeByExtension = await Query()
            .Where(x => x.OwnerId == user.Id && x.UniqueId != user.ProfilePictureId)
            .GroupBy(x => x.Extension)
            .Select(g => new { Extension = g.Key, TotalSize = g.Sum(x => x.Size) })
            .ToDictionaryAsync(x => x.Extension, x => x.TotalSize);

        return Result.Ok(storageSizeByExtension);
    }

    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && !x.IsActive && x.UniqueId != user.ProfilePictureId
        );

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }

    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user)
    {
        var softDeletedMediaResult = await GetSoftDeleted<Media>(user);
        if (softDeletedMediaResult.IsFailed) return softDeletedMediaResult.ToResult<int>();
        
        if (softDeletedMediaResult.Value.Count == 0) return Result.Fail(BinErrors.NoMediaFound());
        
        return await DeleteAllAsync(x => x.OwnerId == user.Id && !x.IsActive && x.UniqueId != user.ProfilePictureId);
    }

    public async Task<Result<Media>> GetByUniqueIdAsync(Guid uniqueId)
    {
        var q = Query().Where(x => x.UniqueId == uniqueId );

        var media = await q.FirstOrDefaultAsync();
        if (media == null)
        {
            return Result.Fail(MediaErrors.InvalidUniqueId(uniqueId.ToString()));
        }

        return Result.Ok(media);
    }

    public async Task<Result<List<TMapped>>> GetRecentlyModified<TMapped>(ApplicationUser user)
    {
        var q = Query().Where(x => x.OwnerId == user.Id && x.IsActive && x.UniqueId != user.ProfilePictureId)
            .OrderByDescending(x => x.UpdatedDate)
            .Take(10);

        var result = await q.FindListAsync<TMapped>("", "UpdatedDate desc");
        return Result.Ok(result);
    }

    public async Task<Result<Dictionary<string, int>>> GetTotalMediaByExtension(ApplicationUser user)
    {
        var result = await Query()
            .Where(x => x.OwnerId == user.Id && x.UniqueId != user.ProfilePictureId)
            .GroupBy(x => x.Extension)
            .Select(g => new { Extension = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Extension, x => x.Count);

        return Result.Ok(result);
    }

    public async Task<Result<int>> SoftDeleteByFolderIdAsync(List<int> folderIds)
    {
        var q = Query().Where(x => x.FolderId.HasValue && folderIds.Contains(x.FolderId.Value) && x.IsActive);

        var affected = await q.ExecuteUpdateAsync(x => x.SetProperty(m => m.IsActive, false));

        return affected;
    }
    
    public async Task<Result<int>> RestoreByFolderIdsAsync(List<int> folderIds)
    {
        var affected = await Query()
            .Where(x => x.FolderId.HasValue && folderIds.Contains(x.FolderId.Value) && !x.IsActive)
            .ExecuteUpdateAsync(x => x.SetProperty(m => m.IsActive, true));

        return affected;
    }
}
