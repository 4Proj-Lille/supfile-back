namespace SupFile.Back.Core.Interfaces.Processors;

public interface IAuthTokenProcessor
{
    (string jwtToken, DateTime expiresAtUtc) GenerateJwtToken(ApplicationUser user);

    (string jwtToken, DateTime expiresAtUtc) GenerateBotJwtToken(ApplicationUser applicationUser);
    (string refreshToken, DateTime expiresAtUtc) GenerateRefreshToken();
    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
}
