using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SupFile.Back.Api.Extensions;

internal static class AuthenticationExtensions
{
    public static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
    {
        var jwtSection = builder.Configuration.GetSection(nameof(JwtSettings));
        builder.Services.Configure<JwtSettings>(jwtSection);
        var jwtSettings = jwtSection.Get<JwtSettings>();

        if (jwtSettings == null)
        {
            throw new InvalidOperationException($"{nameof(JwtSettings)} is null");
        }

        // builder.Services 
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            })
            .AddGoogle(options =>
            {
                var clientId = builder.Configuration["Authentication:Google:ClientId"];
                var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    throw new InvalidOperationException($"Missing configuration value for '{nameof(clientId)}'.");
                }

                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new InvalidOperationException($"Missing configuration value for '{nameof(clientSecret)}'.");
                }

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddMicrosoftAccount(options =>
            {
                var clientId = builder.Configuration["Authentication:Microsoft:ClientId"];
                var clientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    throw new InvalidOperationException($"Missing configuration value for '{nameof(clientId)}'.");
                }

                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new InvalidOperationException($"Missing configuration value for '{nameof(clientSecret)}'.");
                }

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddGitHub(options =>
            {
                var clientId = builder.Configuration["Authentication:GitHub:ClientId"];
                var clientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];

                if (string.IsNullOrWhiteSpace(clientId))
                {
                    throw new InvalidOperationException($"Missing configuration value for '{nameof(clientId)}'.");
                }

                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    throw new InvalidOperationException($"Missing configuration value for '{nameof(clientSecret)}'.");
                }

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Scope.Add("user:email");
                
                options.UsePkce = true;
                options.SaveTokens = true;
                
                options.CorrelationCookie.Name = "__RequestVerificationToken";
                options.CorrelationCookie.HttpOnly = true;
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                
                options.RemoteAuthenticationTimeout = TimeSpan.FromMinutes(15);
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        builder.Services.AddAuthorization();

        return builder;
    }
}
