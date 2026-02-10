using SupFile.Back.Core.Entities.Auth;

namespace SupFile.Back.Data.Repositories;

public class MediaRepository : BaseRepository<Media, int, SupFileContext>, IMediaRepository
{
    public MediaRepository(ILogger<MediaRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<List<TMapped>>> GetFrom<TMapped>(ApplicationUser user, int? id)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.FolderId == id
        ).OrderBy(x => x.Name);

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }
}
