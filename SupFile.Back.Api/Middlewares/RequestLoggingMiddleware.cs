namespace SupFile.Back.Api.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);

            var statusCode = context.Response.StatusCode;
            var logLevel = statusCode switch
            {
                >= 500 => LogLevel.Error,
                >= 400 => LogLevel.Warning,
                _ => LogLevel.Information
            };

            logger.Log(logLevel, "Request: {Method} {Path} - StatusCode: {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                statusCode);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validation failed: {Method} {Path} - {Errors}",
                context.Request.Method,
                context.Request.Path,
                string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            throw;
        }
    }
}
