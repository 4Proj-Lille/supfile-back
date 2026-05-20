using SupFile.Back.Core.Entities.Auth;
using SupFile.Back.Core.Enums;
using SupFile.Back.Data.Context;

namespace SupFile.Back.Data.Repositories;

public class UserRepository : BaseRepository<ApplicationUser, int, SupFileContext>, IUserRepository
{
    public UserRepository(
        ILogger<UserRepository> logger,
        IDbContextFactory<SupFileContext> context) : base(logger, context)
    {
    }
    
    public async Task<Result<List<TMapped>>> GetAllUsersAsync<TMapped>(int currentUserId, int pageNumber, int pageSize)
    {
        var q = Query().Where(x => x.Id != currentUserId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }

    public async Task<Result<TMapped?>> GetUserById<TMapped>(int id)
    {
        var q = Query().Where(x => x.Id == id);
        return Result.Ok(await q.FindByIdAsync<TMapped, int>(id));
    }
    
    public async Task<Result<List<TMapped>>> GetUsersByNameAsync<TMapped>(int currentUserId, string name)
    {
        var q = Query().Where(x => x.Id != currentUserId && EF.Functions.ILike(x.UserName!, $"%{name}%"));
        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }

    public async Task<Result<bool>> DeleteUserAsync(ApplicationUser user)
    {
        await DeleteAsync(user.Id);
        return Result.Ok(true);
    }
    
    public async Task<Result<List<TMapped>>> GetAccessUsersAsync<TMapped>(int objectId, int currentUserId, ObjectType type)
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();

        var sharedUserIds = await ctx.Shares
            .Where(s => type == ObjectType.Media
                ? s.ShareMediaId == objectId
                : s.ShareFolderId == objectId)
            .Select(s => s.UserId)
            .ToListAsync();

        var ownerId = type == ObjectType.Media
            ? await ctx.Medias.Where(m => m.Id == objectId).Select(m => (int?)m.OwnerId).FirstOrDefaultAsync()
            : await ctx.Folders.Where(f => f.Id == objectId).Select(f => (int?)f.OwnerId).FirstOrDefaultAsync();

        var q = Query()
            .Where(x => sharedUserIds.Contains(x.Id) || (ownerId != null && x.Id == ownerId))
            .Where(x => x.Id != currentUserId);

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }
}
