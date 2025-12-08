namespace SupFile.Back.Core.Dto;

public class BotResponseLoginDto
{
    public int Id { get; set; }
    public int userId { get; set; }
    public string? AccessToken { get; set; }
    public DateTime ExpiresAt { get; set; }

    public string? Name { get; set; }
}
