using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using MimeDetective;
using Microsoft.IdentityModel.Tokens;
using SupFile.Back.Core.Errors.Base;

namespace SupFile.Back.Business.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly JwtSettings _jwtSettings;
    private readonly FrontEndSettings _frontEndSettings;
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMediaService _mediaService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthTokenProcessor authTokenProcessor,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IHttpClientFactory httpClientFactory,
        IMediaService mediaService,
        IOptions<AppSettings> appSettings,
        IOptions<FrontEndSettings> frontEndSettings,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _emailService = emailService;
        _httpClientFactory = httpClientFactory;
        _mediaService = mediaService;

        _appSettings = appSettings.Value;
        _frontEndSettings = frontEndSettings.Value;
        _jwtSettings = jwtSettings.Value;

        _logger = logger;
    }

    public async Task<Result<ResponseLoginDto>> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Result.Fail(CommonErrorHelper.NullValue(nameof(refreshToken)));
        }

        var identityUser = await _userManager.GetUserByRefreshTokenAsync(refreshToken);

        if (identityUser == null)
        {
            return Result.Fail(AuthErrors.UserNotFound());
        }

        if (identityUser.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
        {
            return Result.Fail(AuthErrors.RefreshTokenExpired());
        }

        var (jwtToken, jwtExpiresAt) = _authTokenProcessor.GenerateJwtToken(identityUser);
        var (newRefreshToken, newRefreshTokenExpiresAt) = _authTokenProcessor.GenerateRefreshToken();

        identityUser.RefreshToken = newRefreshToken;
        identityUser.RefreshTokenExpiresAtUtc = newRefreshTokenExpiresAt;

        await _userManager.UpdateAsync(identityUser);

        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, jwtExpiresAt);
        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", newRefreshToken,
            newRefreshTokenExpiresAt);

        var responseLoginDto = new ResponseLoginDto
        {
            Id = identityUser.Id,
            AccessToken = jwtToken,
            ExpiresAt = jwtExpiresAt.AddMinutes(-1),
            RefreshToken = newRefreshToken,
            RefreshExpiresAt = newRefreshTokenExpiresAt.AddMinutes(-1),
            Name = identityUser.UserName,
            Email = identityUser.Email
        };

        return Result.Ok(responseLoginDto);
    }

    public async Task<Result> Register(RegisterDto registerDto)
    {
        var userByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
        var userByName = await _userManager.FindByNameAsync(registerDto.Username);

        if (userByEmail is { EmailConfirmed: false })
        {
            await _userManager.DeleteAsync(userByEmail);
            if (userByName?.Id == userByEmail.Id) userByName = null;
            userByEmail = null;
        }

        if (userByName is { EmailConfirmed: false })
        {
            await _userManager.DeleteAsync(userByName);
            userByName = null;
        }

        if (userByEmail != null)
            return Result.Fail(AuthErrors.EmailAlreadyExist());

        if (userByName != null)
            return Result.Fail(AuthErrors.UsernameAlreadyExist());

        var user = new ApplicationUser
        {
            UserName = registerDto.Username.Trim(), Email = registerDto.Email
        };

        var createdResult = await _userManager.CreateAsync(user, registerDto.Password);
        if (!createdResult.Succeeded)
            return ResultHelper.ToFluentResult(createdResult);

        await SendVerificationEmailAsync(user);

        return Result.Ok();
    }

    public async Task<Result<ResponseLoginDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
        {
            return Result.Fail(AuthErrors.InvalidEmailOrPassword());
        }

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result)
        {
            return Result.Fail(AuthErrors.InvalidEmailOrPassword());
        }

        if (_appSettings.RequireEmailVerification && !user.EmailConfirmed)
        {
            return Result.Fail(AuthErrors.EmailNotConfirmed());
        }

        var (jwtToken, expiresAtUtc) = _authTokenProcessor.GenerateJwtToken(user);
        var (refreshToken, refreshExpiresAt) = _authTokenProcessor.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAtUtc = refreshExpiresAt;

        await _userManager.UpdateAsync(user);

        var responseLoginDto = new ResponseLoginDto
        {
            Id = user.Id,
            AccessToken = jwtToken,
            ExpiresAt = expiresAtUtc.AddMinutes(-1),
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshExpiresAt.AddMinutes(-1),
            Name = user.UserName,
            Email = user.Email,
            Language = user.Language
        };

        return Result.Ok(responseLoginDto);
    }


    public async Task<Result> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed
            LogHelper.LogInformation(_logger, nameof(ForgotPasswordAsync),
                "User with email {0} not found or email not confirmed.", forgotPasswordDto.Email);
            return Result.Ok();
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

        return isEmailSent;
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId.ToString());
        if (user == null)
        {
            // Don't reveal that the user does not exist
            LogHelper.LogInformation(_logger, nameof(ResetPasswordAsync), "User with ID {0} not found.",
                resetPasswordDto.UserId);
            return Result.Ok();
        }

        var decodedToken = UrlHelper.UrlDecode(resetPasswordDto.Token);

        var resetPasswordResult =
            await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
        if (!resetPasswordResult.Succeeded)
            return ResultHelper.ToFluentResult(resetPasswordResult);

        return Result.Ok();
    }

    public async Task<Result<ResponseLoginDto>> VerifyEmailAsync(ConfirmEmailDto confirmEmailDto)
    {
        var user = await _userManager.FindByIdAsync(confirmEmailDto.UserId.ToString());
        if (user == null || user.EmailConfirmed)
        {
            LogHelper.LogInformation(_logger, nameof(VerifyEmailAsync),
                "User with ID {0} not found or email already confirmed.", confirmEmailDto.UserId);
            return Result.Fail(AuthErrors.InvalidVerificationToken());
        }

        var decodedToken = WebUtility.UrlDecode(confirmEmailDto.Code);
        var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!confirmEmailResult.Succeeded)
            return ResultHelper.ToFluentResult(confirmEmailResult);

        var (jwtToken, expiresAtUtc) = _authTokenProcessor.GenerateJwtToken(user);
        var (refreshToken, refreshTokenExpiresAtUtc) = _authTokenProcessor.GenerateRefreshToken();

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

    public async Task<Result> ResendVerificationEmailAsync(ResendVerificationEmailDto resendVerificationEmailDto)
    {
        var user = await _userManager.FindByEmailAsync(resendVerificationEmailDto.Email);
        if (user == null || user.EmailConfirmed)
        {
            // Don't reveal that the user does not exist or is already confirmed
            LogHelper.LogInformation(_logger, nameof(ResendVerificationEmailAsync),
                "User with email {0} not found or email already confirmed.", resendVerificationEmailDto.Email);
            return Result.Ok();
        }

        var verificationEmailSentResult = await SendVerificationEmailAsync(user);
        return verificationEmailSentResult;
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
                Username = user.UserName!,
                UserId = user.Id,
                Token = token,
                AppSettings = _appSettings,
                FrontEndSettings = _frontEndSettings
            });

        return isEmailSent;
    }

    public async Task<Result<ResponseLoginDto>> LoginWithProviderAsync(AuthenticateResult result, Providers providerKey)
    {
        var claims = result.Principal!.Claims.ToList();
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var providerId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (email == null || providerId == null)
        {
            return Result.Fail<ResponseLoginDto>("Invalid external data");
        }

        // Check if user exists
        var user = await _userManager.FindByLoginAsync(providerKey.ToString(), providerId);
        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var uniqueUsername = await GenerateUniqueUsernameAsync(name ?? email);
                user = new ApplicationUser { UserName = uniqueUsername, Email = email };
                var userCreatedresult = await _userManager.CreateAsync(user);
                if (!userCreatedresult.Succeeded)
                {
                    var errors = string.Join(", ", userCreatedresult.Errors.Select(e => e.Description));
                    LogHelper.LogInformation(_logger, nameof(LoginWithProviderAsync), "Failed to create user: {0}",
                        errors);
                    return Result.Fail<ResponseLoginDto>(errors);
                }

                // Download and set profile picture from Google on first login
                var pictureUrl = claims.FirstOrDefault(c => c.Type == "picture")?.Value;
                if (!string.IsNullOrEmpty(pictureUrl))
                {
                    var pictureResult = await DownloadAndStoreProfilePictureAsync(user, pictureUrl);
                    if (pictureResult.IsSuccess)
                    {
                        user.ProfilePictureId = pictureResult.Value;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }

            await _userManager.AddLoginAsync(user, new UserLoginInfo(providerKey.ToString(), providerId, "Google"));
        }

        // Generate JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(CustomClaimTypes.UserId, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email!)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);
        var jwtSecurityToken = token as JwtSecurityToken;
        var expiresAtUtc = jwtSecurityToken?.ValidTo ?? DateTime.UtcNow.AddHours(1);

        var (refreshToken, refreshTokenExpiresAtUtc) = _authTokenProcessor.GenerateRefreshToken();

        var responseLoginDto = new ResponseLoginDto
        {
            Id = user.Id,
            AccessToken = jwt,
            ExpiresAt = expiresAtUtc.AddMinutes(-1),
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshTokenExpiresAtUtc.AddMinutes(-1),
            Name = user.UserName,
            Email = user.Email,
            Language = user.Language
        };

        return Result.Ok(responseLoginDto);
    }

    private async Task<string> GenerateUniqueUsernameAsync(string name)
    {
        var sanitized = new string(name
            .Where(c => char.IsLetterOrDigit(c) || c is '-' or '.' or '_' or '@' or '+' or ' ')
            .ToArray());

        if (string.IsNullOrEmpty(sanitized))
            sanitized = "user";

        if (await _userManager.FindByNameAsync(sanitized) == null)
            return sanitized;

        var counter = 2;
        string candidate;
        do
        {
            candidate = $"{sanitized}_{counter++}";
        } while (await _userManager.FindByNameAsync(candidate) != null);

        return candidate;
    }

    private async Task<Result<Guid>> DownloadAndStoreProfilePictureAsync(ApplicationUser user, string pictureUrl)
    {
        try
        {
            if (!Uri.TryCreate(pictureUrl, UriKind.Absolute, out var uri))
                return Result.Fail<Guid>("Invalid profile picture URL");

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
                return Result.Fail<Guid>("Failed to download profile picture");

            var bytes = await response.Content.ReadAsByteArrayAsync();

            var inspector = new ContentInspectorBuilder()
            {
                Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
            }.Build();
            var results = inspector.Inspect(bytes);
            var mimeType = results.ByMimeType().FirstOrDefault()?.MimeType
                ?? response.Content.Headers.ContentType?.MediaType
                ?? "image/jpeg";
            var ext = results.ByFileExtension().FirstOrDefault()?.Extension is { } e ? $".{e}" : ".jpg";

            var mediaResult = await _mediaService.AddOneAsync(user, bytes, $"profile{ext}", mimeType);
            if (mediaResult.IsFailed)
                return mediaResult.ToResult();

            return Result.Ok(mediaResult.Value.UniqueId);
        }
        catch (Exception ex)
        {
            LogHelper.LogError(_logger, nameof(DownloadAndStoreProfilePictureAsync),
                ex, "Error downloading profile picture from {0}", pictureUrl);
            return Result.Fail<Guid>("Error downloading profile picture");
        }
    }
}
