using System.Transactions;
using SupFile.Back.Core.Enums;

namespace SupFile.Back.Business.Services;

public class UserService : BaseService<ApplicationUser, int, IUserRepository>, IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediaService _mediaService;

    public UserService(ILogger<UserService> logger, IUserRepository repository, IMediaService mediaService,
         UserManager<ApplicationUser> userManager) :
        base(logger, repository)
    {
        _userManager = userManager;
        _mediaService = mediaService;
    }
    
    public async Task<Result<ApplicationUser>> AddUser(ApplicationUser user)
    {
        var result = await Repository.AddAsync(user);
        return result;
    }
    
    
    public async Task<Result<List<TMapped>>> GetAllUsersAsync<TMapped>(ApplicationUser currentUser, int pageNumber,
        int pageSize)
    {
        // Add permissions
        var users = await Repository.GetAllUsersAsync<TMapped>(currentUser.Id, pageNumber, pageSize);
        return users;
    }
    
    private static readonly HashSet<string> s_includeProps =
    [
        nameof(ApplicationUser.Theme),
        nameof(ApplicationUser.Language),
        nameof(ApplicationUser.DisplayName),
    ];
    
    public async Task<Result<ApplicationUser>> UpdateAsync(int userId, ApplicationUser entity, ApplicationUser currentUser, IFormFile? profilePicture)
    {
        if (currentUser.Id != userId)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<ApplicationUser, int>(userId));
        }
        
        var userResult = await Repository.GetByIdAsync<ApplicationUser>(userId);
        if (userResult.IsFailed || userResult.Value == null)
        {
            return Result.Fail(AuthErrors.UserNotFound());
        }

        var user = userResult.Value;
        
        foreach (var prop in typeof(ApplicationUser).GetProperties())
        {
            if (!s_includeProps.Contains(prop.Name))
                continue;
            
            if (!ScalarTypeHelper.IsScalarProperty(prop))
            {
                continue;
            }

            var value = prop.GetValue(entity);
            if (value != null)
            {
                prop.SetValue(user, value);
            }
        }
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        if (profilePicture != null && MediaTypeHelper.Resolve(Path.GetExtension(profilePicture.FileName)) == "Picture")
        {
            var mediaResult = await _mediaService.AddOneAsync(user, profilePicture);
            if (mediaResult.IsFailed)
            {
                return Result.Fail(mediaResult.Errors);
            }

            user.ProfilePictureId = mediaResult.Value.UniqueId;
        }
        
        var result = await Repository.UpdateAsync(userId, user);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }
        
        scope.Complete();

        return result;
    }
    
    public async Task<Result<ApplicationUser>> UpdatePasswordAsync(int userId, string currentPassword,
        string newPassword, string confirmNewPassword, ApplicationUser currentUser)
    {
        if (currentUser.Id != userId)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<ApplicationUser, int>(userId));
        }

        if (newPassword != confirmNewPassword)
        {
            return Result.Fail(UserErrors.PasswordDoesntMatch());
        }
        
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Fail(AuthErrors.UserNotFound());
        }
        
        var passwordCheck = await _userManager.CheckPasswordAsync(user, currentPassword);
        if (!passwordCheck)
        {
            return Result.Fail(UserErrors.IncorrectPassword());
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
        {
            return Result.Ok(user);
        }

        return Result.Fail(result.Errors.Select(e => e.Description).ToList());
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser currentUser, int userId)
    {
        var userResult = await Repository.GetUserById<ApplicationUser>(userId);
        if (userResult.IsFailed || userResult.Value == null)
        {
            return Result.Fail(AuthErrors.UserNotFound());
        }

        var user = userResult.Value;
        
        if (currentUser.Id != userId)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<ApplicationUser, int>(userId));
        }
        
        var result = await Repository.DeleteUserAsync(user);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }

        var aspNetUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (aspNetUser == null)
        {
            return Result.Fail(UserErrors.AspNetUserNotFound());
        }

        await _userManager.DeleteAsync(aspNetUser);

        return Result.Ok();
    }
    
    public async Task<Result<List<TMapped>>> GetAccessUsersAsync<TMapped>(int id, ApplicationUser currentUser, ObjectType type)
    {
        var usersResult = await Repository.GetAccessUsersAsync<TMapped>(id, currentUser.Id, type);
        if (usersResult.IsFailed)
        {
            return Result.Fail(UserErrors.AspNetUserNotFound());
        }

        return usersResult;
    }

}