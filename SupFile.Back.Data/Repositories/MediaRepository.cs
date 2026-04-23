using SupFile.Back.Core.Entities.Auth;
using SupFile.Back.Core.Errors;

namespace SupFile.Back.Data.Repositories;

public class MediaRepository : BaseRepository<Media, int, SupFileContext>, IMediaRepository
{
    public MediaRepository(ILogger<MediaRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? folderId, string filter)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.FolderId == folderId && x.IsActive
        );

        var result = await q.FindListAsync<TMapped>(filter);
        return Result.Ok(result);
    }

    public async Task<Result<Dictionary<string, int>>> GetTotalStorageSize(ApplicationUser user)
    {
        return Result.Ok(
            new Dictionary<string, int>
            {
                ["Global"] = await Query()
                    .Where(x => x.OwnerId == user.Id)
                    .SumAsync(x => x.Size)
            }
        );
    }

    public async Task<Result<Dictionary<string, int>>> GetStorageSizeByExtension(ApplicationUser user)
    {
        var storageSizeByExtension = await Query()
            .Where(x => x.OwnerId == user.Id)
            .GroupBy(x => x.Extension)
            .Select(g => new { Extension = g.Key, TotalSize = g.Sum(x => x.Size) })
            .ToDictionaryAsync(x => x.Extension, x => x.TotalSize);

        return Result.Ok(storageSizeByExtension);
    }

    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && !x.IsActive
        );

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }

    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user)
    {
        return await DeleteAllAsync(x => x.OwnerId == user.Id && !x.IsActive);
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
        var q = Query().Where(x => x.OwnerId == user.Id && x.IsActive)
            .OrderByDescending(x => x.UpdatedDate)
            .Take(10);

        var result = await q.FindListAsync<TMapped>("", "UpdatedDate desc");
        return Result.Ok(result);
    }
    
    public async Task<Result<int>> GetTotalMediaByType(ApplicationUser currentUser, string filter)
    {
        var q = Query().Where(x => x.OwnerId == currentUser.Id);

        if (!string.IsNullOrWhiteSpace(filter))
            q = q.ApplyFiltering(filter);

        var count = await q.CountAsync();
        return Result.Ok(count);
    }
}
