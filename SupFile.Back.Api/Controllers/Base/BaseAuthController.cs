namespace SupFile.Back.Api.Controllers.Base;

[ApiController]
[Authorize]
public abstract class BaseAuthController : BaseController
{
    private readonly IUserRepository _userRepository;

    protected BaseAuthController(
        ILogger<BaseAuthController> logger,
        IUserRepository userRepository,
        IHostEnvironment environment
    ) :
        base(logger, environment)
    {
        _userRepository = userRepository;
    }

    protected async Task<ApplicationUser> GetAuthenticatedAppUserAsync()
    {
        var stringUserId = User.FindFirst(CustomClaimTypes.UserId)?.Value;
        if (string.IsNullOrEmpty(stringUserId))
        {
            throw new Exception("User not found");
        }

        if (!int.TryParse(stringUserId, out var userId))
        {
            throw new Exception("Malformed user id");
        }

        var user = await _userRepository.FindOneAsync<ApplicationUser>(x => x.Id == userId);
        if (user.IsFailed || user.Value == null)
        {
            throw new Exception("User not found");
        }

        return user.Value;
    }
}
