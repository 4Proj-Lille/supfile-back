namespace SupFile.Back.Api.Controllers.Base;

[ApiController]
[Authorize]
public abstract class BaseAuthController : BaseController
{
    private readonly UserManager<AuthIdentityUser> _userManager;
    private readonly IUserRepository _userRepository;

    protected BaseAuthController(
        ILogger<BaseAuthController> logger,
        UserManager<AuthIdentityUser> userManager,
        IUserRepository userRepository,
        IHostEnvironment environment
    ) :
        base(logger, environment)
    {
        _userManager = userManager;
        _userRepository = userRepository;
    }

    protected async Task<AuthIdentityUser> GetAuthenticatedUserIdentityAsync()
    {
        var userId = User.FindFirst(CustomClaimTypes.IdentityId)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User not found");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        return user;
    }

    protected async Task<ApplicationUser> GetAuthenticatedAppUserAsync()
    {
        var stringUserId = User.FindFirst(CustomClaimTypes.ApplicationUserId)?.Value;
        if (string.IsNullOrEmpty(stringUserId))
        {
            throw new Exception("User not found");
        }

        if (!int.TryParse(stringUserId, out var userId))
        {
            throw new Exception("Malformed user id");
        }

        var user = await _userRepository.FindOneAsync<ApplicationUser>(x => x.Id == userId);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        return user;
    }
}
