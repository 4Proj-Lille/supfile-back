namespace SupFile.Back.Api.Controllers.Auth;

[Route("api/[controller]")]
public sealed class AccountController : BaseController
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        IAuthService authService,
        ILogger<AccountController> logger,
        IWebHostEnvironment env) : base(logger, env)
    {
        _authService = authService;
        _userManager = userManager;
    }

    // POST: /Account/Register
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto model,
        [FromServices] IValidator<RegisterDto> validator)
    {
        await validator.ValidateAndThrowAsync(model);

        var result = await _authService.Register(model);
        if (result.IsFailed) return ToErrorActionResult(result);

        return Created();
    }

    [HttpGet("Me")]
    [Authorize]
    public async Task<IActionResult> GetOwnUser()
    {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null)
        {
            return Unauthorized();
        }

        var user = await _userManager.Users
            // .Include(u => u.ApplicationUser)
            .FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user is null)
        {
            return Unauthorized();
        }

        var userModel = user.Adapt<ApplicationUserModel>();
        return Ok(userModel);
    }
}
