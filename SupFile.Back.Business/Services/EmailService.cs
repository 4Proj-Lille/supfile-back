using Microsoft.AspNetCore.Hosting;

namespace SupFile.Back.Business.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IFluentEmail _fluentEmail;
    private readonly IWebHostEnvironment _env;

    public EmailService(IFluentEmail fluentEmail, IWebHostEnvironment env, ILogger<EmailService> logger)
    {
        _fluentEmail = fluentEmail;
        _env = env;
        _logger = logger;
    }

    public async Task<Result> SendEmailAsync(string receipientEmail, string subject, string templateName, object model)
    {
        try
        {
            const string TemplateFolder = "EmailTemplates";
            LogHelper.LogInformation(_logger, nameof(SendEmailAsync), "Email '{0}' sending to '{1}'.", templateName,
                receipientEmail);
            var templatePath = Path.Combine(_env.ContentRootPath, TemplateFolder, templateName);

            await _fluentEmail
                .To(receipientEmail)
                .Subject(subject)
                .UsingTemplateFromFile(templatePath, model, true)
                .SendAsync();

            LogHelper.LogInformation(_logger, nameof(SendEmailAsync), "Email '{0}' sent to '{1}'.", templateName,
                receipientEmail);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            LogHelper.LogError(_logger, nameof(SendEmailAsync), ex, "Email '{0}' to '{1}' could not be sent.",
                templateName, receipientEmail);
            return Result.Fail(new ExceptionalError(ex));
        }
    }
}
