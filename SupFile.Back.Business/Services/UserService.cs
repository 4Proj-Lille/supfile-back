namespace SupFile.Back.Business.Services;

public class UserService : BaseService<ApplicationUser, int, IUserRepository>, IUserService
{
    private readonly UserManager<AuthIdentityUser> _userManager;

    public UserService(ILogger<UserService> logger, IUserRepository repository,
         UserManager<AuthIdentityUser> userManager) :
        base(logger, repository)
    {
        _userManager = userManager;
    }
    
    public async Task<Result<ApplicationUser>> AddUser(ApplicationUser user)
    {
        var result = await Repository.AddAsync(user);
        return result;
    }
    
    public async Task<Result> DeleteUserAsync(ApplicationUser currentUser, int userId)
    {
        var userResult = await Repository.GetUserById<ApplicationUser>(userId);
        if (userResult.IsFailed || userResult.Value == null)
        {
            return Result.Fail(new NotFoundError($"The user with id {userId} not found"));
        }

        var user = userResult.Value;

        if (!user.IdentityUserId.HasValue)
        {
            return Result.Fail(
                new NotFoundError("The requested user is a bot, bots cannot be deleted by this endpoint."));
        }

        // Can delete own user, modify permissions
        bool canDelete = currentUser.Id == userId;
        
        if (!canDelete)
        {
            return Result.Fail(
                new ForbiddenError($"The user with id {currentUser.Id} can't delete the user with id {userId}"));
        }
        
        var result = await Repository.DeleteUserAsync(user);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        var aspNetUser = await _userManager.FindByIdAsync(user.IdentityUserId.Value.ToString());
        if (aspNetUser == null)
        {
            return Result.Fail(new NotFoundError($"The AspNetUser with id {user.IdentityUserId} not found"));
        }

        await _userManager.DeleteAsync(aspNetUser);

        return Result.Ok();
    }

    
    public async Task<Result<List<TMapped>>> GetAllUsersAsync<TMapped>(ApplicationUser currentUser, int pageNumber,
        int pageSize)
    {
        // Add permissions
        var users = await Repository.GetAllUsersAsync<TMapped>(currentUser.Id, pageNumber, pageSize);
        return users;
    }

}