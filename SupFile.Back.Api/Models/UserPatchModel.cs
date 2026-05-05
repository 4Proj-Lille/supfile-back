namespace SupFile.Back.Api.Models;

public class UserPatchModel
{
    public string UserName { get; set; }
    
    public UserLanguage Language { get; set; }
    
    public UserTheme Theme { get; set; }
}
