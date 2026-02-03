namespace SupFile.Back.Business.Models.Templates.Emails;

public class ForgotPasswordEmailModel
{
    public string UserName { get; set; }
    public string EncodedToken { get; set; }
    public int UserId { get; set; }
    public AppSettings AppSettings { get; set; }
    public FrontEndSettings FrontEndSettings { get; set; }

    public string ResetLink =>
        $"{FrontEndSettings.BaseUrl}{FrontEndSettings.ResetPasswordLink}?userId={UserId}code={EncodedToken}";
}
