namespace SupFile.Back.Business.Models.Templates.Emails;

public class VerificationEmailModel
{
    public string Username { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public AppSettings AppSettings { get; set; }
    public FrontEndSettings FrontEndSettings { get; set; }
    
    public string VerificationLink =>
        $"{FrontEndSettings.BaseUrl}{FrontEndSettings.EmailVerificationLink}?userId={UserId}code={Token}";
}
