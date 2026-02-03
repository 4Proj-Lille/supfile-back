using SupFile.Back.Core.Dto;

namespace SupFile.Back.Core.Interfaces.Services.Auth;

public interface IAuthService
{
    // public Task<bool> HasGlobalPermission(ApplicationUser user, string permissionName);
    public Task<Result<bool>> Register(RegisterDto registerDto);
    public Task<Result<ResponseLoginDto>> Login(LoginDto loginDto);
    // public Task<Result<ResponseLoginDto>> LoginWithProviderAsync(ClaimsPrincipal claimsPrincipal, Providers provider);
    public Task<Result<ResponseLoginDto>> RefreshTokenAsync(string refreshToken);
    
    public Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    public Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    
    // ResendVerificationEmailAsync
    public Task<Result<ResponseLoginDto>> VerifyEmailAsync(ConfirmEmailDto confirmEmailDto);
    public Task<Result<bool>> ResendVerificationEmailAsync(ResendVerificationEmailDto resendVerificationEmailDto);
}
