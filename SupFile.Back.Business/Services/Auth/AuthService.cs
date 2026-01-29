using Microsoft.Extensions.Hosting;

namespace SupFile.Back.Business.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    public AuthService(
        IAuthTokenProcessor authTokenProcessor,
        UserManager<ApplicationUser> userManager,
        IOptions<AppSettings> appSettings,
        IEmailService emailService
    )
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _appSettings = appSettings.Value;
        _emailService = emailService;
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

        // var applicationUser =
        //     await _userRepository.FindOneAsync<AuthIdentityUser>(x => x.IdentityUserId == identityUser.Id);
        // if (applicationUser == null)
        // {
        //     return Result.Fail(new BadRequestError("Application user not found"));
        // }

        var (jwtToken, expirationDateInUtc) = _authTokenProcessor.GenerateJwtToken(identityUser);
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
            Name = identityUser.UserName,
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

        // var appUser = await _userRepository.FindOneAsync<ApplicationUser>(x => x.Username == registerDto.UserName);
        // if (appUser != null)
        // {
        //     return Result.Fail(new ConflictError("A user with the same username already exists."));
        // }

        identityUser = new ApplicationUser { UserName = registerDto.UserName, Email = registerDto.Email };

        var result = await _userManager.CreateAsync(identityUser, registerDto.Password);

        Console.WriteLine(result.Errors);
        if (!result.Succeeded)
        {
            return Result.Fail(new BadRequestError(result.Errors.First().Description));
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
        var verificationUrl = new Uri(QueryHelpers.AddQueryString(
            callbackBaseUrl.ToString(),
            new Dictionary<string, string?>
            {
                { "userId", WebUtility.UrlEncode(identityUser.Id.ToString()) },
                { "code", WebUtility.UrlEncode(token) }
            }
        ));


        var (template, subject) = EmailTemplateConstants.VerifyEmail;
        var isEmailSent = await _emailService.SendEmailAsync(
            identityUser.Email,
            subject,
            template,
            new VerificationEmailModel
            {
                UserName = identityUser.UserName!,
                VerificationUrl = verificationUrl,
                AppSettings = _appSettings,
                BaseUrl = callbackBaseUrl,
            });

        if (isEmailSent.IsFailed)
        {
            return Result.Fail<bool>(isEmailSent.Errors);
        }

        return Result.Ok(true);
    }

    // public async Task<Result<ResponseLoginDto>> LoginWithProviderAsync(ClaimsPrincipal claimsPrincipal,
    //     Providers provider)
    // {
    //     var providerString = provider.ToString();
    //     var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
    //     if (string.IsNullOrEmpty(email))
    //     {
    //         return Result.Fail(new BadRequestError("Email is null or empty"));
    //     }
    //
    //     var identityUser = await _userManager.FindByEmailAsync(email);
    //
    //     // if the user is not found, register the user
    //     if (identityUser == null)
    //     {
    //         var username = email.Split("@")[0] ?? email;
    //         var newIdentityUser = new AuthIdentityUser
    //         {
    //             UserName = username,
    //             Email = email,
    //             EmailConfirmed = true,
    //             ApplicationUser = new ApplicationUser
    //             {
    //                 Username = username,
    //                 FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? username,
    //                 LastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname)
    //             }
    //         };
    //
    //         var result = await _userManager.CreateAsync(newIdentityUser);
    //         if (!result.Succeeded)
    //         {
    //             return Result.Fail(new BadRequestError(
    //                 $"Unable to create user {string.Join(", ", result.Errors.Select(e => e.Description))}"));
    //         }
    //
    //         var createdUser = await _userManager.FindByEmailAsync(email);
    //         if (createdUser == null || createdUser.ApplicationUser == null)
    //         {
    //             return Result.Fail(new BadRequestError("Unable to find created user"));
    //         }
    //
    //         // var addUserRoleResult = await _generalRoleService.AddUserRole(createdUser.ApplicationUser.Id);
    //         // if (addUserRoleResult.IsFailed)
    //         // {
    //         //     return Result.Fail<ResponseLoginDto>(addUserRoleResult.Errors);
    //         // }
    //
    //         identityUser = createdUser;
    //     }
    //
    //     var logins = await _userManager.GetLoginsAsync(identityUser);
    //     var alreadyLinked = logins.Any(login => login.LoginProvider == providerString);
    //
    //     if (!alreadyLinked)
    //     {
    //         var info = new UserLoginInfo(
    //             providerString,
    //             claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
    //             providerString);
    //
    //
    //         var loginResult = await _userManager.AddLoginAsync(identityUser, info);
    //         if (!loginResult.Succeeded)
    //         {
    //             return Result.Fail(new BadRequestError(
    //                 $"Unable to link external login {string.Join(", ", loginResult.Errors.Select(e => e.Description))}"));
    //         }
    //     }
    //
    //     var applicationUser =
    //         await _userRepository.FindOneAsync<ApplicationUser>(x => x.IdentityUserId == identityUser.Id);
    //     if (applicationUser == null)
    //     {
    //         return Result.Fail(new BadRequestError("Application user not found"));
    //     }
    //
    //     var (jwtToken, expiresAtUtc) = _authTokenProcessor.GenerateJwtToken(identityUser, applicationUser);
    //     var refreshToken = _authTokenProcessor.GenerateRefreshToken();
    //
    //     var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);
    //
    //     identityUser.RefreshToken = refreshToken;
    //     identityUser.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;
    //
    //     await _userManager.UpdateAsync(identityUser);
    //
    //     _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expiresAtUtc);
    //     _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", refreshToken, refreshTokenExpiresAtUtc);
    //
    //     var responseLoginDto = new ResponseLoginDto
    //     {
    //         Id = identityUser.Id,
    //         AccessToken = jwtToken,
    //         ExpiresAt = expiresAtUtc.AddMinutes(-1),
    //         RefreshToken = refreshToken,
    //         RefreshExpiresAt = refreshTokenExpiresAtUtc.AddMinutes(-1),
    //         Name = identityUser.UserName,
    //         Email = identityUser.Email
    //     };
    //
    //     return Result.Ok(responseLoginDto);
    // }

    public async Task<Result<ResponseLoginDto>> Login(LoginDto loginDto)
    {
        const string ErrorMessage = "Invalid email or password";
        var identityUser = await _userManager.FindByEmailAsync(loginDto.Email);
        if (identityUser == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessage));
        }

        var result = await _userManager.CheckPasswordAsync(identityUser, loginDto.Password);
        if (!result)
        {
            return Result.Fail(new BadRequestError(ErrorMessage));
        }

        if (_appSettings.RequireEmailVerification && !identityUser.EmailConfirmed)
        {
            return Result.Fail(new BadRequestError("Email not confirmed"));
        }

        var (jwtToken, expiresAtUtc) = _authTokenProcessor.GenerateJwtToken(identityUser);
        var refreshToken = _authTokenProcessor.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        identityUser.RefreshToken = refreshToken;
        identityUser.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;

        await _userManager.UpdateAsync(identityUser);

        var responseLoginDto = new ResponseLoginDto
        {
            Id = identityUser.Id,
            AccessToken = jwtToken,
            ExpiresAt = expiresAtUtc.AddMinutes(-1),
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshTokenExpiresAtUtc.AddMinutes(-1),
            Name = identityUser.UserName,
            Email = identityUser.Email,
            Language = identityUser.Language
        };

        return Result.Ok(responseLoginDto);
    }
}
