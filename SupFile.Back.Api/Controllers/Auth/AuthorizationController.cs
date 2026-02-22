using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SupFile.Back.Api.Controllers.Auth;

[Route("api/[controller]")]
public sealed class AuthorizationController : BaseController
{
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;


    public AuthorizationController(
        IAuthService authService,
        IAuthTokenProcessor authTokenProcessor,
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthorizationController> logger,
        IWebHostEnvironment env
    ) : base(logger, env)
    {
        _authService = authService;
        _userManager = userManager;
        _authTokenProcessor = authTokenProcessor;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("login")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<ActionResult<ResponseLoginDto>> Login([FromForm] LoginDto loginDto,
        [FromServices] IValidator<LoginDto> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, loginDto);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        var responseLoginDto = await _authService.Login(loginDto);

        return ToActionResult(responseLoginDto);
    }

    [HttpPost("login/refreshtoken")]
    public async Task<ActionResult<ResponseLoginDto>> RefreshToken([FromQuery] string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return ToActionResult(Result.Fail(new BadRequestError("Refresh token is missing.")));
        }

        var responseLoginDto = await _authService.RefreshTokenAsync(refreshToken);

        return ToActionResult(responseLoginDto);
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<ResponseLoginDto>> ConfirmEmail(
        [FromBody] ConfirmEmailDto model,
        [FromServices] IValidator<ConfirmEmailDto> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, model);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        var responseLoginDto = await _authService.VerifyEmailAsync(model);

        return ToActionResult(responseLoginDto);
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail(
        [FromForm] ResendVerificationEmailDto resendVerificationEmailDto,
        [FromServices] IValidator<ResendVerificationEmailDto> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, resendVerificationEmailDto);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        var responseDto = await _authService.ResendVerificationEmailAsync(resendVerificationEmailDto);
        return ToActionResult(responseDto.ToResult());
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDto forgotPasswordDto,
        [FromServices] IValidator<ForgotPasswordDto> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, forgotPasswordDto);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        var responseForgotPasswordDto = await _authService.ForgotPasswordAsync(forgotPasswordDto);
        return ToActionResult(responseForgotPasswordDto.ToResult());
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto,
        [FromServices] IValidator<ResetPasswordDto> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, resetPasswordDto);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        var responseResetPasswordDto = await _authService.ResetPasswordAsync(resetPasswordDto);

        return ToActionResult(responseResetPasswordDto.ToResult());
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin([FromQuery] string returnUrl)
    {
        if(string.IsNullOrEmpty(returnUrl))
        {
            return ToActionResult(Result.Fail(new BadRequestError("ReturnUrl is missing."))); 
        }
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleResponse), null, new { returnUrl }, Request.Scheme)
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google/callback")]
    public async Task<ActionResult<ResponseLoginDto>> GoogleResponse([FromQuery] string returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!result.Succeeded)
            return BadRequest("External authentication failed");

        var loginResponseResult = await _authService.LoginWithProviderAsync(result, Providers.Google);
        
        // append token or session id if needed
        var urlWithToken = $"{returnUrl}?token={loginResponseResult.Value.AccessToken}&refreshToken={loginResponseResult.Value.RefreshToken}";

        return Redirect(urlWithToken);
        
        // return ToActionResult(loginResponseResult);
    }
}
