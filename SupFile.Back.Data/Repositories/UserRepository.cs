using SupFile.Back.Core.Entities.Auth;

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

    public async Task<Result<bool>> DeleteUserAsync(ApplicationUser user)
    {
        await DeleteAsync(user.Id);
        return Result.Ok(true);
    }
}
