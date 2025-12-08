namespace SupFile.Back.Api.Models;

public class ApplicationUserModel
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    
    public string Username { get; set; }

    public UserLanguage Language { get; set; }

    public string LanguageLocalized
    {
        get => Language.ToLocalizedEnum();
    }

    public Guid? ProfilePictureId { get; set; }
}
