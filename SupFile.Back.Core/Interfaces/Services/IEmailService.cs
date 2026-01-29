namespace SupFile.Back.Core.Interfaces.Services;

public interface IEmailService
{
    Task<Result> SendEmailAsync(string receipientEmail, string subject, string templateName, object model);
}
