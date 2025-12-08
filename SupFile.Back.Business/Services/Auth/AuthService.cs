namespace SupFile.Back.Business.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly IFluentEmail _fluentEmail;
    private readonly UserManager<AuthIdentityUser> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;

    public AuthService(
        IAuthTokenProcessor authTokenProcessor,
        UserManager<AuthIdentityUser> userManager,
        IUserRepository userRepository,
        IOptions<AppSettings> appSettings,
        IFluentEmail fluentEmail,
        IUserService userService
    )
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
        _appSettings = appSettings.Value;
        _fluentEmail = fluentEmail;
        _userService = userService;
    }

    public async Task<Result<ResponseLoginDto>> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Result.Fail(new BadRequestError("Refresh token is missing."));
        }

        var identityUser = await _userManager.GetUserByRefreshTokenAsync(refreshToken);

        if (identityUser == null)
        {
            return Result.Fail(new BadRequestError("Unable to retrieve user for refresh token"));
        }

        if (identityUser.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
        {
            return Result.Fail(new BadRequestError("Refresh token is expired."));
        }

        var applicationUser =
            await _userRepository.FindOneAsync<ApplicationUser>(x => x.IdentityUserId == identityUser.Id);
        if (applicationUser == null)
        {
            return Result.Fail(new BadRequestError("Application user not found"));
        }

        var (jwtToken, expirationDateInUtc) = _authTokenProcessor.GenerateJwtToken(identityUser, applicationUser);
        var refreshTokenValue = _authTokenProcessor.GenerateRefreshToken();

        var refreshTokenExpirationDateInUtc = DateTime.UtcNow.AddDays(7);

        identityUser.RefreshToken = refreshTokenValue;
        identityUser.RefreshTokenExpiresAtUtc = refreshTokenExpirationDateInUtc;

        await _userManager.UpdateAsync(identityUser);

        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);
        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", identityUser.RefreshToken,
            refreshTokenExpirationDateInUtc);

        var responseLoginDto = new ResponseLoginDto
        {
            Id = identityUser.Id,
            AccessToken = jwtToken,
            ExpiresAt = expirationDateInUtc.AddMinutes(-1),
            RefreshToken = refreshTokenValue,
            RefreshExpiresAt = refreshTokenExpirationDateInUtc.AddMinutes(-1),
            Name = applicationUser.Username,
            Email = identityUser.Email
        };

        return Result.Ok(responseLoginDto);
    }

    public async Task<Result<bool>> Register(RegisterDto registerDto, Uri callbackBaseUrl)
    {
        var identityUser = await _userManager.FindByEmailAsync(registerDto.Email)
                           ?? await _userManager.FindByNameAsync(registerDto.UserName);

        //if user already exists but email is not confirmed, override it
        if (identityUser is { EmailConfirmed: false })
        {
            await _userManager.DeleteAsync(identityUser);
            identityUser = null;
        }

        if (identityUser != null)
        {
            var conflictDetail = identityUser.Email == registerDto.Email
                ? "A user with the same email already exists."
                : "A user with the same username already exists.";

            return Result.Fail(new ConflictError(conflictDetail));
        }

        var appUser = await _userRepository.FindOneAsync<ApplicationUser>(x => x.Username == registerDto.UserName);
        if (appUser != null)
        {
            return Result.Fail(new ConflictError("A user with the same username already exists."));
        }

        identityUser = new AuthIdentityUser { UserName = registerDto.UserName, Email = registerDto.Email };

        var result = await _userManager.CreateAsync(identityUser, registerDto.Password);

        Console.WriteLine(result.Errors);
        if (!result.Succeeded)
        {
            return Result.Fail(new BadRequestError(result.Errors.First().Description));
        }

        var newUser = new ApplicationUser
        {
            FirstName = registerDto.UserName,
            IdentityUserId = identityUser.Id,
            Username = registerDto.UserName,
            Language = UserLanguage.English
        };
            
        var addUserResult = await _userService.AddUser(newUser);
        if (addUserResult.IsFailed)
        {
            return Result.Fail<bool>(addUserResult.Errors);
        }

        // var addUserRoleResult = await _generalRoleService.AddUserRole(newUser.Id);
        //
        // if (addUserRoleResult.IsFailed)
        // {
        //     return Result.Fail<bool>(addUserRoleResult.Errors);
        // }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
        var verificationUrl = new Uri(QueryHelpers.AddQueryString(
            callbackBaseUrl.ToString(),
            new Dictionary<string, string?>
            {
                { "userId", WebUtility.UrlEncode(identityUser.Id.ToString()) },
                { "code", WebUtility.UrlEncode(token) }
            }
        ));

        try
        {
            await _fluentEmail
                .To(identityUser.Email)
                .Subject($"Email verification for {_appSettings.Name}")
                .Body(
                    $"<h2>{identityUser.UserName}</h2>Please click <a href=\"{verificationUrl}\">here</a> to verify your email address.",
                    true)
                // .UsingTemplateFromFile("EmailTemplates/VerificationEmail.cshtml", emailModel)
                .SendAsync();
        }
        catch (Exception ex)
        {
            return Result.Fail(new BadRequestError(ex.Message));
        }

        return Result.Ok(true);
    }

    public async Task<Result<ResponseLoginDto>> LoginWithProviderAsync(ClaimsPrincipal claimsPrincipal,
        Providers provider)
    {
        var providerString = provider.ToString();
        var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            return Result.Fail(new BadRequestError("Email is null or empty"));
        }

        var identityUser = await _userManager.FindByEmailAsync(email);

        // if the user is not found, register the user
        if (identityUser == null)
        {
            var username = email.Split("@")[0] ?? email;
            var newIdentityUser = new AuthIdentityUser
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                ApplicationUser = new ApplicationUser
                {
                    Username = username,
                    FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? username,
                    LastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname)
                }
            };

            var result = await _userManager.CreateAsync(newIdentityUser);
            if (!result.Succeeded)
            {
                return Result.Fail(new BadRequestError(
                    $"Unable to create user {string.Join(", ", result.Errors.Select(e => e.Description))}"));
            }

            var createdUser = await _userManager.FindByEmailAsync(email);
            if (createdUser == null || createdUser.ApplicationUser == null)
            {
                return Result.Fail(new BadRequestError("Unable to find created user"));
            }

            // var addUserRoleResult = await _generalRoleService.AddUserRole(createdUser.ApplicationUser.Id);
            // if (addUserRoleResult.IsFailed)
            // {
            //     return Result.Fail<ResponseLoginDto>(addUserRoleResult.Errors);
            // }

            identityUser = createdUser;
        }

        var logins = await _userManager.GetLoginsAsync(identityUser);
        var alreadyLinked = logins.Any(login => login.LoginProvider == providerString);

        if (!alreadyLinked)
        {
            var info = new UserLoginInfo(
                providerString,
                claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                providerString);


            var loginResult = await _userManager.AddLoginAsync(identityUser, info);
            if (!loginResult.Succeeded)
            {
                return Result.Fail(new BadRequestError(
                    $"Unable to link external login {string.Join(", ", loginResult.Errors.Select(e => e.Description))}"));
            }
        }

        var applicationUser =
            await _userRepository.FindOneAsync<ApplicationUser>(x => x.IdentityUserId == identityUser.Id);
        if (applicationUser == null)
        {
            return Result.Fail(new BadRequestError("Application user not found"));
        }

        var (jwtToken, expiresAtUtc) = _authTokenProcessor.GenerateJwtToken(identityUser, applicationUser);
        var refreshToken = _authTokenProcessor.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        identityUser.RefreshToken = refreshToken;
        identityUser.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;

        await _userManager.UpdateAsync(identityUser);

        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expiresAtUtc);
        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", refreshToken, refreshTokenExpiresAtUtc);

        var responseLoginDto = new ResponseLoginDto
        {
            Id = identityUser.Id,
            AccessToken = jwtToken,
            ExpiresAt = expiresAtUtc.AddMinutes(-1),
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshTokenExpiresAtUtc.AddMinutes(-1),
            Name = identityUser.UserName,
            Email = identityUser.Email
        };

        return Result.Ok(responseLoginDto);
    }

    public async Task<Result<ResponseLoginDto>> Login(LoginDto loginDto)
    {
        const string ErrorMessage = "Invalid email or password";
        var identityUser = await _userManager.FindByEmailAsync(loginDto.Email);
        if (identityUser == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessage));
        }

        if (_appSettings.RequireEmailVerification && !identityUser.EmailConfirmed)
        {
            return Result.Fail(new BadRequestError("Email not confirmed"));
        }


        var applicationUser =
            await _userRepository.FindOneAsync<ApplicationUser>(x => x.IdentityUserId == identityUser.Id);
        if (applicationUser == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessage));
        }

        var result = await _userManager.CheckPasswordAsync(identityUser, loginDto.Password);
        if (!result)
        {
            return Result.Fail(new BadRequestError(ErrorMessage));
        }

        var token = _authTokenProcessor.GenerateJwtToken(identityUser, applicationUser);
        var refreshToken = _authTokenProcessor.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateAsync(applicationUser.Id, applicationUser);

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
            Language = applicationUser.Language
        };

        return Result.Ok(responseLoginDto);
    } 
}
