using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SupFile.Back.Core.Configuration;
using SupFile.Back.Core.Entities.Auth;
using SupFile.Back.Core.Extensions;

namespace SupFile.Back.Data.Seeds;

public class UsersSeeder
{
    private readonly AppSettings _appSettings;
    private readonly UserManager<AuthIdentityUser> _userManager;

    public UsersSeeder(UserManager<AuthIdentityUser> userManager, IOptions<AppSettings> appSettings)
    {
        ArgumentNullException.ThrowIfNull(appSettings);

        _userManager = userManager;
        _appSettings = appSettings.Value;
    }

    public async Task SeedAsync(SupFileContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var user = new AuthIdentityUser
        {
            UserName = _appSettings.DefaultUserEmail,
            Email = _appSettings.DefaultUserEmail,
            EmailConfirmed = true,
            ApplicationUser = new ApplicationUser
            {
                Username = _appSettings.DefaultUserEmail,
                FirstName = _appSettings.DefaultUserEmail,
                Language = UserLanguageExtensions.FromCultureCode(_appSettings.SupportedCultures.First())
            }
        };

        await _userManager.CreateAsync(user, _appSettings.DefaultUserPassword);
    }
}
