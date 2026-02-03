namespace SupFile.Back.Api.Models;

public class ApplicationUserModel
{
    public int Id { get; set; }
    
    public string UserName { get; set; }
    
    public UserLanguage Language { get; set; }
    
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? ConcurrencyStamp { get; set; }
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }


    public string LanguageLocalized
    {
        get => Language.ToLocalizedEnum();
    }

    public Guid? ProfilePictureId { get; set; }
}
