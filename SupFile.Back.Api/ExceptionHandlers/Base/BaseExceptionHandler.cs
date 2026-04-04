using Microsoft.AspNetCore.Diagnostics;
using SupFile.Back.Api.Helpers;

namespace SupFile.Back.Api.ExceptionHandlers.Base;

internal abstract class BaseExceptionHandler<TException> : IExceptionHandler
    where TException : Exception
{
    protected readonly IProblemDetailsService ProblemDetailsService;
    private readonly ILogger _logger;

    protected BaseExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger logger
    )
    {
        ProblemDetailsService = problemDetailsService;
        _logger = logger;
    }

    protected abstract int StatusCode { get; }
    protected abstract string Title { get; }
    protected virtual LogLevel LogLevel => LogLevel.Error;
    protected abstract string GetDetail(TException exception);

    public virtual async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not TException typedException)
            return false;

        LogException(httpContext, exception);

        httpContext.Response.StatusCode = StatusCode;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCode,
            Type = ProblemDetailsUriHelper.GetProblemTypeUri(StatusCode),
            Title = Title,
            Detail = GetDetail(typedException),
        };

        if (await ProblemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext, Exception = exception, ProblemDetails = problemDetails,
            }))
        {
            return true;
        }

        _logger.LogWarning("ProblemDetailsService failed to write. Falling back to direct JSON response.");
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private void LogException(HttpContext httpContext, Exception exception)
    {
        _logger.Log(
            LogLevel,
            exception,
            "Unhandled exception. Path: {Path} | Method: {Method} | Status: {StatusCode} | " +
            "Type: {ExceptionType} | TraceId: {TraceId} | User: {User} | Source: {Source}",
            httpContext.Request.Path,
            httpContext.Request.Method,
            StatusCode,
            exception.GetType().Name,
            httpContext.TraceIdentifier,
            httpContext.User.Identity?.Name ?? "Anonymous",
            exception.Source);
    }
}
