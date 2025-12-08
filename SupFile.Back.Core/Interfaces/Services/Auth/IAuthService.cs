using System.Security.Claims;
using SupFile.Back.Core.Dto;
using SupFile.Back.Core.Enums.Auth;

namespace SupFile.Back.Core.Interfaces.Services.Auth;

public interface IAuthService
{
    // public Task<bool> HasGlobalPermission(ApplicationUser user, string permissionName);
    public Task<Result<bool>> Register(RegisterDto registerDto, Uri callbackBaseUrl);
    public Task<Result<ResponseLoginDto>> Login(LoginDto loginDto);
    public Task<Result<ResponseLoginDto>> LoginWithProviderAsync(ClaimsPrincipal claimsPrincipal, Providers provider);
    public Task<Result<ResponseLoginDto>> RefreshTokenAsync(string refreshToken);
}
