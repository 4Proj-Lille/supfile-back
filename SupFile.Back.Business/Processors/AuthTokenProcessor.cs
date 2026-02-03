using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SupFile.Back.Core.Extensions;

namespace SupFile.Back.Business.Processors;

public class AuthTokenProcessor : IAuthTokenProcessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSettings _jwtSettings;

    public AuthTokenProcessor(IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtSettings = jwtSettings.Value;
    }

    public (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(ApplicationUser user)
    {
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        if (string.IsNullOrEmpty(user.Email))
        {
            throw new Exception("Email is empty");
        }

        var claims = new[]
        {
            new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Locality, user.Language.ToCultureCode()), // e.g., "fr-FR"
        };

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeInMinutes);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: expires,
            signingCredentials: credentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (jwtToken, expires);
    }

    public (string jwtToken, DateTime expiresAtUtc) GenerateBotJwtToken(ApplicationUser applicationUser)
    {
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(CustomClaimTypes.UserId, applicationUser.Id.ToString(CultureInfo.InvariantCulture))
        };

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeInMinutes);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: expires,
            signingCredentials: credentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        return (jwtToken, expires);
    }


    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token,
        DateTime expiration)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieName,
            token, new CookieOptions
            {
                HttpOnly = false,
                Expires = expiration,
                IsEssential = true,
                // Secure = _httpContextAccessor.HttpContext?.Request.IsHttps ?? false,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
    }
}
