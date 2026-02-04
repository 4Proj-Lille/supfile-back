using Microsoft.AspNetCore.Authentication;
using SupFile.Back.Core.Dto;
using SupFile.Back.Core.Enums.Auth;

namespace SupFile.Back.Core.Interfaces.Services.Auth;

public interface IAuthService
{
    // public Task<bool> HasGlobalPermission(ApplicationUser user, string permissionName);
    public Task<Result<bool>> Register(RegisterDto registerDto);
    public Task<Result<ResponseLoginDto>> Login(LoginDto loginDto);
    public Task<Result<ResponseLoginDto>> RefreshTokenAsync(string refreshToken);

    public Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    public Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    public Task<Result<ResponseLoginDto>> VerifyEmailAsync(ConfirmEmailDto confirmEmailDto);
    public Task<Result<bool>> ResendVerificationEmailAsync(ResendVerificationEmailDto resendVerificationEmailDto);

    public Task<Result<ResponseLoginDto>> LoginWithProviderAsync(AuthenticateResult result, Providers providerKey);
}
