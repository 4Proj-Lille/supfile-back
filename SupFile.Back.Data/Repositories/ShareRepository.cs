using SupFile.Back.Core.Entities.Auth;

namespace SupFile.Back.Data.Repositories;

public class ShareRepository : BaseRepository<Share, int, SupFileContext>, IShareRepository
{
    public ShareRepository(ILogger<ShareRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<List<TMapped>>> GetAllFoldersSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        var q = ctx.Folders
            .Where(f => f.IsActive &&
                        ctx.Shares.Any(s => s.UserId == user.Id && s.ShareFolderId == f.Id));

        var result = await q.FindListAsync<TMapped>(filter, orderBy: orderBy);
        return Result.Ok(result);
    }

    public async Task<Result<List<TMapped>>> GetAllMediasSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        var q = ctx.Medias
            .Where(m => m.IsActive &&
                        ctx.Shares.Any(s => s.UserId == user.Id && s.ShareMediaId == m.Id));

        var result = await q.FindListAsync<TMapped>(filter, orderBy: orderBy);
        return Result.Ok(result);
    }
}
