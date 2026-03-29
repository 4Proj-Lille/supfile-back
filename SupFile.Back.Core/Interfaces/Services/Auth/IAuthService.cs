using Microsoft.AspNetCore.Authentication;
using SupFile.Back.Core.Dto;
using SupFile.Back.Core.Enums.Auth;

namespace SupFile.Back.Core.Interfaces.Services.Auth;

public interface IAuthService
{
    public Task<Result> Register(RegisterDto registerDto);
    public Task<Result<ResponseLoginDto>> Login(LoginDto loginDto);
    public Task<Result<ResponseLoginDto>> RefreshTokenAsync(string refreshToken);

    public Task<Result> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    public Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    public Task<Result<ResponseLoginDto>> VerifyEmailAsync(ConfirmEmailDto confirmEmailDto);
    public Task<Result> ResendVerificationEmailAsync(ResendVerificationEmailDto resendVerificationEmailDto);

    public Task<Result<ResponseLoginDto>> LoginWithProviderAsync(AuthenticateResult result, Providers providerKey);
}
