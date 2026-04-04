using SupFile.Back.Core.Entities.Auth;
using System.Linq.Dynamic.Core;

namespace SupFile.Back.Data.Repositories;

public class MediaRepository : BaseRepository<Media, int, SupFileContext>, IMediaRepository
{
    public MediaRepository(ILogger<MediaRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<List<TMapped>>> GetFrom<TMapped>(ApplicationUser user, int? folderId, string sort)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.FolderId == folderId
        );
        try
        {
            var result = await q.FindListAsync<TMapped>("", sort);
            return Result.Ok(result);
        }
        catch(GridifyMapperException ex)
        {
            return Result.Fail<List<TMapped>>(ex.Message);
        }
    }

    public async Task<Result<int>> GetGlobalStorage(ApplicationUser user)
    {
        return Result.Ok(
            await Query()
                .Where(x => x.OwnerId == user.Id)
                .SumAsync(x => x.Size)
        );
    }

    public async Task<Result<Dictionary<string, int>>> GetStorageByExtension(ApplicationUser user)
    {
        var storageByExtension = await Query()
            .Where(x => x.OwnerId == user.Id)
            .GroupBy(x => x.Extension)
            .Select(g => new { Extension = g.Key, TotalSize = g.Sum(x => x.Size) })
            .ToDictionaryAsync(x => x.Extension, x => x.TotalSize);

        return Result.Ok(storageByExtension);
    }

    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.IsActive
        );

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }

    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user)
    {
        return await DeleteAllAsync(x => x.OwnerId == user.Id && !x.IsActive);
    }
}
