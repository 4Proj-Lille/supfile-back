using Microsoft.AspNetCore.Diagnostics;

namespace SupFile.Back.Api.Middleware;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment environment;
    private readonly ILogger<GlobalExceptionHandler> logger;
    private readonly IProblemDetailsService problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService,
        IHostEnvironment environment
    )
    {
        this.logger = logger;
        this.problemDetailsService = problemDetailsService;
        this.environment = environment;
    }

    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "An unhandled exception occurred while processing the request.");

        var message = environment.IsDevelopment()
            ? exception.Message
            : "An error occurred while processing your request. Please try again";
        return problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Title = "Internal Server Error", Detail = message }
        });
    }
}
