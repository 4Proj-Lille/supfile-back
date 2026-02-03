namespace SupFile.Back.Business.Extensions;

public static class UserManagerExtensions
{
    public static async Task<ApplicationUser?> GetUserByRefreshTokenAsync(
        this UserManager<ApplicationUser> userManager,
        string refreshToken)
    {
            return await userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}
