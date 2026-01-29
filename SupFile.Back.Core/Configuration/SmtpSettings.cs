namespace SupFile.Back.Core.Configuration;

public class SmtpSettings
{
    [Required]
    public string Server { get; set; }
    [Required]
    public int Port { get; set; }
    [Required]
    public bool UseSsl { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string MailFrom { get; set; }
    [Required]
    public string MailFromDisplayName { get; set; }
}
