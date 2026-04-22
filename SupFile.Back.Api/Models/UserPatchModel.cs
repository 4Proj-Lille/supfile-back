namespace SupFile.Back.Api.Models;

public class UserPatchModel
{
    public string DisplayName { get; set; }
    
    public UserLanguage Language { get; set; }
    
    public UserTheme Theme { get; set; }

    public Guid? ProfilePictureId { get; set; }
}
