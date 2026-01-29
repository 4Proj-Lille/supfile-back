namespace SupFile.Back.Api.Controllers.Auth;

[Route("api/[controller]")]
public sealed class AccountController : BaseController
{
    private readonly AppSettings _appSettings;
    private readonly IAuthService _authService;
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;

    public AccountController(
        IAuthTokenProcessor authTokenProcessor,
        UserManager<ApplicationUser> userManager,
        IUserService userService,
        IAuthService authService,
        ILogger<AccountController> logger,
        IWebHostEnvironment env,
        IOptions<AppSettings> appSettings) : base(logger, env)
    {
        _authService = authService;
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userService = userService;
        _appSettings = appSettings.Value;
    }

    // POST: /Account/Register
    [HttpPost("register")]
    public async Task<ActionResult<bool>> Register([FromBody] RegisterDto model,
        [FromServices] IValidator<RegisterDto> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, model);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        // Generate the callback URL endpoint
        var emailConfirmCallbackUrl = _appSettings.EmailVerificationFrontendLink;

        var result = await _authService.Register(model, new Uri(emailConfirmCallbackUrl));
        if (result.IsFailed)
        {
            return ToActionResult(result);
        }

        return Created();
    }

    [HttpPatch("{userId}/confirmEmail")]
    public async Task<ActionResult<ResponseLoginDto>> ConfirmEmail(string userId,
        [FromBody] ConfirmEmailDto model,
        [FromServices] IValidator<ConfirmEmailDto> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, model);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        var decodedUserId = WebUtility.UrlDecode(userId);
        var identityUser = await _userManager.FindByIdAsync(decodedUserId);
        if (identityUser == null)
        {
            return NotFound();
        }

        if (identityUser.EmailConfirmed)
        {
            return BadRequest("Email already confirmed.");
        }

        var decodedToken = WebUtility.UrlDecode(model.Code);
        var result = await _userManager.ConfirmEmailAsync(identityUser, decodedToken);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // var applicationUserResult =
        //     await _userService.GetOneAsync<ApplicationUser>(x => x.IdentityUserId == identityUser.Id);
        // if (applicationUserResult.IsFailed || applicationUserResult.Value == null)
        // {
        //     return ToActionResult(Result.Fail(applicationUserResult.Errors));
        // }

        // var applicationUser = applicationUserResult.Value;

        var token = _authTokenProcessor.GenerateJwtToken(identityUser);
        var refreshToken = _authTokenProcessor.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        identityUser.RefreshToken = refreshToken;
        identityUser.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;

        await _userManager.UpdateAsync(identityUser);

        var responseLoginDto = new ResponseLoginDto
        {
            Id = identityUser.Id,
            AccessToken = token.jwtToken,
            ExpiresAt = token.expiresAtUtc.AddMinutes(-1),
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshTokenExpiresAtUtc.AddMinutes(-1),
            Name = identityUser.UserName,
            Email = identityUser.Email,
            Language = identityUser.Language
        };

        return Ok(responseLoginDto);
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
