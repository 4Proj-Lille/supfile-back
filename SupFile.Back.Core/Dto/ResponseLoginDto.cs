namespace SupFile.Back.Core.Dto;

public class ResponseLoginDto
{
    public Guid Id { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime RefreshExpiresAt { get; set; }

    public string? Name { get; set; }
    public string? Email { get; set; }
    public UserLanguage Language { get; set; }
}
