using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SupFile.Back.Api.Extensions;

internal static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        var authProviderSettings =
            builder.Configuration.GetSection(nameof(AuthProviderSettings)).Get<AuthProviderSettings>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // API requests
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Google login
                options.DefaultSignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme; // Temporary storage for OAuth
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                // options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddGoogle(nameof(Providers.Google), options =>
            {
                options.ClientId = authProviderSettings.Google.ClientId;
                options.ClientSecret = authProviderSettings.Google.ClientSecret;
                options.CallbackPath = "/auth/google/callback";
                options.SaveTokens = true;
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}
