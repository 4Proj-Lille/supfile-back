namespace SupFile.Back.Api.Controllers.Auth;

[Route("api/[controller]")]
public sealed class AuthorizationController : BaseController
{
    private readonly IAuthService _authService;
    private readonly LinkGenerator _linkGenerator;

    public AuthorizationController(
        IAuthService authService,
        ILogger<AuthorizationController> logger,
        LinkGenerator linkGenerator,
        IWebHostEnvironment env
    ) : base(logger, env)
    {
        _authService = authService;
        _linkGenerator = linkGenerator;
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

    // [HttpGet("login/{provider}")]
    // public IResult LoginUsingProvider([FromRoute] string provider, [FromQuery] Uri? returnUrl)
    // {
    //     var providerMap = new Dictionary<string, string>
    //     {
    //         ["GOOGLE"] = "Google", ["MICROSOFT"] = "Microsoft", ["GITHUB"] = "GitHub"
    //     };
    //
    //     if (!providerMap.TryGetValue(provider.ToUpperInvariant(), out var scheme))
    //     {
    //         return Results.BadRequest($"Unsupported provider: {provider}");
    //     }
    //
    //     var redirectUrl = _linkGenerator.GetPathByRouteValues(
    //         HttpContext,
    //         null,
    //         new { controller = "Authorization", action = nameof(LoginUsingProviderCallback), provider, returnUrl }
    //     );
    //
    //     var properties = _signInManager.ConfigureExternalAuthenticationProperties(scheme, redirectUrl);
    //
    //     return Results.Challenge(properties, [scheme]);
    // }

    // [HttpGet("login/{provider}/callback")]
    // public async Task<IActionResult> LoginUsingProviderCallback([FromRoute] string provider,
    //     [FromQuery] Uri? returnUrl)
    // {
    //     var providerMap = new Dictionary<string, (string Scheme, Providers InternalName)>
    //     {
    //         ["GOOGLE"] = (GoogleDefaults.AuthenticationScheme, Providers.Google),
    //         ["MICROSOFT"] = (MicrosoftAccountDefaults.AuthenticationScheme, Providers.Microsoft),
    //         ["GITHUB"] = (GitHubAuthenticationDefaults.AuthenticationScheme, Providers.GitHub)
    //     };
    //
    //     if (!providerMap.TryGetValue(provider.ToUpperInvariant(), out var info))
    //     {
    //         return BadRequest($"Unsupported provider: {provider}");
    //     }
    //
    //     var claimResult = await HttpContext.AuthenticateAsync(info.Scheme);
    //     if (!claimResult.Succeeded)
    //     {
    //         return Unauthorized();
    //     }
    //
    //     var tokenResult = await _authService.LoginWithProviderAsync(claimResult.Principal, info.InternalName);
    //
    //     var uriBuilder = new UriBuilder(returnUrl ?? new Uri("/"));
    //     var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
    //     query["ACCESS_TOKEN"] = tokenResult.Value.AccessToken;
    //     query["REFRESH_TOKEN"] = tokenResult.Value.RefreshToken;
    //     uriBuilder.Query = query.ToString();
    //
    //     return Redirect((uriBuilder).ToString());
    // }

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
}
