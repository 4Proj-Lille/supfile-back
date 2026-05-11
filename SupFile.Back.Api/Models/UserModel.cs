namespace SupFile.Back.Api.Models;

public class UserModel
{
    public int Id { get; set; }
    
    public string UserName { get; set; }
    
    public string Email { get; set; }
    
    public UserLanguage Language { get; set; }
        
    public UserTheme Theme { get; set; }
    
    public Guid? ProfilePictureId { get; set; }

}
