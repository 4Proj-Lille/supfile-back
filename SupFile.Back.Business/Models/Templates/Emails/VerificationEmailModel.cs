namespace SupFile.Back.Business.Models.Templates.Emails;

public class VerificationEmailModel
{
    public Uri VerificationUrl { get; set; }
    public string UserName { get; set; }

    public Uri BaseUrl { get; set; }

    public AppSettings AppSettings { get; set; }
}
