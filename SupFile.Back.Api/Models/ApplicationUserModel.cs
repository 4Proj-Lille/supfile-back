namespace SupFile.Back.Api.Models;

public class ApplicationUserModel
{
    public int Id { get; set; }
    
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    
    public string? Email { get; set; }
    public UserTheme Theme { get; set; }

    public Guid? ProfilePictureId { get; set; }
}
