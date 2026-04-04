namespace SupFile.Back.Api.Controllers.Auth;

[Route("api/[controller]")]
public sealed class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger,
        IWebHostEnvironment env
    ) : base(logger, env)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<ActionResult<ResponseLoginDto>> Login([FromForm] LoginDto loginDto,
        [FromServices] IValidator<LoginDto> validator)
    {
        // throw new NotImplementedException();
        await validator.ValidateAndThrowAsync(loginDto);

        var responseLoginDto = await _authService.Login(loginDto);
        return ToOkActionResult(responseLoginDto);
    }

    [HttpPost("login/refreshtoken")]
    public async Task<ActionResult<ResponseLoginDto>> RefreshToken([FromQuery] string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return ToErrorActionResult(Result.Fail(CommonErrorHelper.NullValue(nameof(refreshToken))));
        }

        var responseLoginDto = await _authService.RefreshTokenAsync(refreshToken);
        return ToOkActionResult(responseLoginDto);
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult<ResponseLoginDto>> ConfirmEmail(
        [FromBody] ConfirmEmailDto model,
        [FromServices] IValidator<ConfirmEmailDto> validator)
    {
        await validator.ValidateAndThrowAsync(model);

        var responseLoginDto = await _authService.VerifyEmailAsync(model);

        return ToOkActionResult(responseLoginDto);
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail(
        [FromForm] ResendVerificationEmailDto resendVerificationEmailDto,
        [FromServices] IValidator<ResendVerificationEmailDto> validator)
    {
        await validator.ValidateAndThrowAsync(resendVerificationEmailDto);

        var responseDto = await _authService.ResendVerificationEmailAsync(resendVerificationEmailDto);
        return ToNoContentActionResult(responseDto);
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDto forgotPasswordDto,
        [FromServices] IValidator<ForgotPasswordDto> validator)
    {
        await validator.ValidateAndThrowAsync(forgotPasswordDto);

        var responseForgotPasswordDto = await _authService.ForgotPasswordAsync(forgotPasswordDto);
        return ToNoContentActionResult(responseForgotPasswordDto);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto,
        [FromServices] IValidator<ResetPasswordDto> validator)
    {
        await validator.ValidateAndThrowAsync(resetPasswordDto);

        var responseResetPasswordDto = await _authService.ResetPasswordAsync(resetPasswordDto);
        return ToNoContentActionResult(responseResetPasswordDto);
    }

    [HttpGet("google")]
    public IActionResult GoogleLogin([FromQuery] string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            return ToErrorActionResult(Result.Fail(CommonErrorHelper.NullValue(nameof(returnUrl))));
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
        var urlWithToken =
            $"{returnUrl}?token={loginResponseResult.Value.AccessToken}&refreshToken={loginResponseResult.Value.RefreshToken}";

        return Redirect(urlWithToken);
    }
}
