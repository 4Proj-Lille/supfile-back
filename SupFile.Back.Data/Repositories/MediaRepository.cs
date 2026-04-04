using SupFile.Back.Core.Entities.Auth;
using System.Linq.Dynamic.Core;

namespace SupFile.Back.Data.Repositories;

public class MediaRepository : BaseRepository<Media, int, SupFileContext>, IMediaRepository
{
    public MediaRepository(ILogger<MediaRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? folderId,
        string sort)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.FolderId == folderId
        );

        var result = await q.FindListAsync<TMapped>("", sort);
        return Result.Ok(result);
    }

    public async Task<Result<int>> GetTotalStorageSize(ApplicationUser user)
    {
        return Result.Ok(
            await Query()
                .Where(x => x.OwnerId == user.Id)
                .SumAsync(x => x.Size)
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
}
