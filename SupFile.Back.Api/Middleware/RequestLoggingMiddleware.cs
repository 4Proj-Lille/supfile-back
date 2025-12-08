namespace SupFile.Back.Api.Middleware;

internal sealed class RequestLoggingMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestTime = DateTime.UtcNow;
        var endpoint = context.Request.Path;

        try
        {
            await _next(context); // proceed to the next middleware/controller
            _logger.LogInformation("Request: {Time} {Endpoint} - Status: {StatusCode}",
                requestTime, endpoint, context.Response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: {Time} {Endpoint} - Exception: {Message}",
                requestTime, endpoint, ex.Message);
            throw; // rethrow so it still hits your exception handler
        }
    }
}
