namespace SupFile.Back.Core.Interfaces.Processors;

public interface IAuthTokenProcessor
{
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(AuthIdentityUser identityUser,
        ApplicationUser applicationUser);

    (string jwtToken, DateTime expiresAtUtc) GenerateBotJwtToken(ApplicationUser applicationUser);
    string GenerateRefreshToken();
    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
}
