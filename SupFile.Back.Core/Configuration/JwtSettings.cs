namespace SupFile.Back.Core.Configuration;

public class JwtSettings
{
    [Required] public required string Secret { get; set; }

    [Required] public required string Issuer { get; set; }

    [Required] public required string Audience { get; set; }

    [Required] public required int ExpirationTimeInMinutes { get; set; }
}
