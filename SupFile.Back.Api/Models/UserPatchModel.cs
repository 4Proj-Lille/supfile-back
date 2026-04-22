namespace SupFile.Back.Api.Models;

public class UserPatchModel
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    
    public UserLanguage Language { get; set; }
    
    public string? PhoneNumber { get; set; }

    public Guid? ProfilePictureId { get; set; }
}
