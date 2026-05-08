using SupFile.Back.Core.Entities.Auth;
using SupFile.Back.Core.Enums;
using SupFile.Back.Core.Errors;

namespace SupFile.Back.Data.Repositories;

public class ShareRepository : BaseRepository<Share, int, SupFileContext>, IShareRepository
{
    public ShareRepository(ILogger<ShareRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<List<TMapped>>> GetAllFoldersSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy, int? limit = null)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        var q = ctx.Folders
            .Where(f => f.IsActive &&
                        ctx.Shares.Any(s => s.UserId == user.Id && s.ShareFolderId == f.Id));
        
        if (limit.HasValue)
            q = q.Take(limit.Value);
        
        var result = await q.FindListAsync<TMapped>(filter, orderBy: orderBy);
        return Result.Ok(result);
    }

    public async Task<Result<List<TMapped>>> GetAllMediasSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy, int? limit = null)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        var q = ctx.Medias
            .Where(m => m.IsActive &&
                        ctx.Shares.Any(s => s.UserId == user.Id && s.ShareMediaId == m.Id));

        if (limit.HasValue)
            q = q.Take(limit.Value);
        
        var result = await q.FindListAsync<TMapped>(filter, orderBy: orderBy);
        return Result.Ok(result);
    }

    public async Task<bool> HasMediaPermissionAsync(int mediaId, int userId, SharePermission permission)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        return await ctx.Shares.AnyAsync(s =>
            s.ShareMediaId == mediaId &&
            s.UserId == userId &&
            s.Permission == permission.ToString());
    }

    public async Task<bool> HasFolderPermissionAsync(int folderId, int userId, SharePermission permission)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        return await ctx.Shares.AnyAsync(s =>
            s.ShareFolderId == folderId &&
            s.UserId == userId &&
            s.Permission == permission.ToString());
    }

    public async Task<Result<Share>> GetByObjectAndUserAsync(int objectId, int userId, InvitationItemType type)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        var share = type == InvitationItemType.Media
            ? await ctx.Shares.FirstOrDefaultAsync(s => s.ShareMediaId == objectId && s.UserId == userId)
            : await ctx.Shares.FirstOrDefaultAsync(s => s.ShareFolderId == objectId && s.UserId == userId);

        if (share == null)
            return Result.Fail(ShareErrors.ShareNotFound());

        return Result.Ok(share);
    }
}
