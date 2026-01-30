namespace SupFile.Back.Business.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly FrontEndSettings _frontEndSettings;
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthTokenProcessor authTokenProcessor,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IOptions<AppSettings> appSettings,
        IOptions<FrontEndSettings> frontEndSettings,
        ILogger<AuthService> logger)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _emailService = emailService;

        _appSettings = appSettings.Value;
        _frontEndSettings = frontEndSettings.Value;

        _logger = logger;
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

    public async Task<Result<bool>> Register(RegisterDto registerDto)
    {
        var user = await _userManager.FindByEmailAsync(registerDto.Email)
                   ?? await _userManager.FindByNameAsync(registerDto.UserName);

        //if user already exists but email is not confirmed, override it
        if (user is { EmailConfirmed: false })
        {
            await _userManager.DeleteAsync(user);
            user = null;
        }

        if (user != null)
        {
            var conflictDetail = user.Email == registerDto.Email
                ? "A user with the same email already exists."
                : "A user with the same username already exists.";

            return Result.Fail(new ConflictError(conflictDetail));
        }

        // var appUser = await _userRepository.FindOneAsync<ApplicationUser>(x => x.Username == registerDto.UserName);
        // if (appUser != null)
        // {
        //     return Result.Fail(new ConflictError("A user with the same username already exists."));
        // }

        user = new ApplicationUser { UserName = registerDto.UserName, Email = registerDto.Email };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        Console.WriteLine(result.Errors);
        if (!result.Succeeded)
        {
            return Result.Fail(new BadRequestError(result.Errors.First().Description));
        }

        await SendVerificationEmailAsync(user);

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
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessage));
        }

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result)
        {
            return Result.Fail(new BadRequestError(ErrorMessage));
        }

        if (_appSettings.RequireEmailVerification && !user.EmailConfirmed)
        {
            return Result.Fail(new BadRequestError("Email not confirmed"));
        }

        var (jwtToken, expiresAtUtc) = _authTokenProcessor.GenerateJwtToken(user);
        var refreshToken = _authTokenProcessor.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;

        await _userManager.UpdateAsync(user);

        var responseLoginDto = new ResponseLoginDto
        {
            Id = user.Id,
            AccessToken = jwtToken,
            ExpiresAt = expiresAtUtc.AddMinutes(-1),
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshTokenExpiresAtUtc.AddMinutes(-1),
            Name = user.UserName,
            Email = user.Email,
            Language = user.Language
        };

        return Result.Ok(responseLoginDto);
    }


    public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            LogHelper.LogInformation(_logger, nameof(ForgotPasswordAsync),
                "User with email {0} not found or email not confirmed.", forgotPasswordDto.Email);
            return Result.Ok(true);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = UrlHelper.UrlEncode(token);

        var (template, subject) = EmailTemplateConstants.ForgotPassword;
        var isEmailSent = await _emailService.SendEmailAsync(
            user.Email,
            subject,
            template,
            new ForgotPasswordEmailModel
            {
                UserName = user.UserName!,
                EncodedToken = encodedToken,
                UserId = user.Id,
                AppSettings = _appSettings,
                FrontEndSettings = _frontEndSettings
            });

        if (isEmailSent.IsFailed)
        {
            return Result.Fail<bool>(isEmailSent.Errors);
        }

        return Result.Ok(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId.ToString());
        if (user == null)
        {
            // Don't reveal that the user does not exist
            LogHelper.LogInformation(_logger, nameof(ResetPasswordAsync), "User with ID {0} not found.",
                resetPasswordDto.UserId);
            return Result.Ok(true);
        }

        var decodedToken = UrlHelper.UrlDecode(resetPasswordDto.Token);

        var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            return Result.Fail(new BadRequestError(result.Errors.First().Description));
        }

        return Result.Ok(true);
    }

    public async Task<Result<ResponseLoginDto>> VerifyEmailAsync(ConfirmEmailDto confirmEmailDto)
    {
        var user = await _userManager.FindByIdAsync(confirmEmailDto.UserId.ToString());
        if (user == null || user.EmailConfirmed)
        {
            // Don't reveal that the user does not exist or is already confirmed
            LogHelper.LogInformation(_logger, nameof(VerifyEmailAsync),
                "User with ID {0} not found or email already confirmed.", confirmEmailDto.UserId);
            return Result.Ok();
        }

        var decodedToken = WebUtility.UrlDecode(confirmEmailDto.Code);
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            return Result.Fail(new BadRequestError(result.Errors.First().Description));
        }

        var (jwtToken, expiresAtUtc) = _authTokenProcessor.GenerateJwtToken(user);
        var refreshToken = _authTokenProcessor.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;

        await _userManager.UpdateAsync(user);

        var responseLoginDto = new ResponseLoginDto
        {
            Id = user.Id,
            AccessToken = jwtToken,
            ExpiresAt = expiresAtUtc.AddMinutes(-1),
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshTokenExpiresAtUtc.AddMinutes(-1),
            Name = user.UserName,
            Email = user.Email,
            Language = user.Language
        };

        return Result.Ok(responseLoginDto);
    }

    public async Task<Result<bool>> ResendVerificationEmailAsync(ResendVerificationEmailDto resendVerificationEmailDto)
    {
        var user = await _userManager.FindByEmailAsync(resendVerificationEmailDto.Email);
        if (user == null || user.EmailConfirmed)
        {
            // Don't reveal that the user does not exist or is already confirmed
            LogHelper.LogInformation(_logger, nameof(ResendVerificationEmailAsync),
                "User with email {0} not found or email already confirmed.", resendVerificationEmailDto.Email);
            return Result.Ok(true);
        }

        var verificationEmailSentResult = await SendVerificationEmailAsync(user);
        return verificationEmailSentResult.IsSuccess;
    }

    private async Task<Result> SendVerificationEmailAsync(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var (template, subject) = EmailTemplateConstants.VerifyEmail;
        var isEmailSent = await _emailService.SendEmailAsync(
            user.Email,
            subject,
            template,
            new VerificationEmailModel
            {
                UserName = user.UserName!,
                UserId = user.Id,
                Token = token,
                AppSettings = _appSettings,
                FrontEndSettings = _frontEndSettings
            });

        if (isEmailSent.IsFailed)
        {
            return Result.Fail(isEmailSent.Errors);
        }

        return Result.Ok();
    }
}
