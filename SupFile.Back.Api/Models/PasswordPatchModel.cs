namespace SupFile.Back.Api.Models;

public class PasswordPatchModel
{
    public string CurrentPassword { get; set; } = null!;
    
    public string NewPassword { get; set; } = null!;
    
    public string ConfirmNewPassword { get; set; } = null!;
}