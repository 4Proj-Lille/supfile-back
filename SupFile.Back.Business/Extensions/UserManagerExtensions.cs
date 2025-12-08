namespace SupFile.Back.Business.Extensions;

public static class UserManagerExtensions
{
    public static async Task<AuthIdentityUser?> GetUserByRefreshTokenAsync(
        this UserManager<AuthIdentityUser> userManager,
        string refreshToken)
    {
            return await userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}
