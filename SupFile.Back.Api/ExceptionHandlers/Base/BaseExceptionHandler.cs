using Microsoft.AspNetCore.Diagnostics;
using SupFile.Back.Api.Helpers;

namespace SupFile.Back.Api.ExceptionHandlers.Base;

internal abstract class BaseExceptionHandler<TException> : IExceptionHandler
    where TException : Exception
{
    protected readonly IProblemDetailsService ProblemDetailsService;
    protected readonly Microsoft.Extensions.Logging.ILogger Logger;

    protected BaseExceptionHandler(
        IProblemDetailsService problemDetailsService,
        Microsoft.Extensions.Logging.ILogger logger
    )
    {
        ProblemDetailsService = problemDetailsService;
        Logger = logger;
    }

    protected abstract int StatusCode { get; }
    protected abstract string Title { get; }
    protected abstract string Detail { get; }
    protected virtual LogLevel LogLevel => LogLevel.Error;
    protected virtual string GetDetail(TException exception) => Detail;

    public virtual async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not TException typedException)
            return false;

        const string UnhandledException = "Unhandled exception occurred. Path: {Path}, Method: {Method}, StatusCode: {StatusCode}, ExceptionType: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}, TraceId: {TraceId}, User: {User}, InnerException: {InnerException}, Source: {Source}";

        Logger.Log(
                LogLevel,
                exception,
                UnhandledException,
                httpContext.Request.Path,
                httpContext.Request.Method,
                StatusCode,
                exception.GetType().Name,
                exception.Message,
                exception.StackTrace,
                httpContext.TraceIdentifier,
                httpContext.User?.Identity?.Name ?? "Anonymous",
                exception.InnerException?.ToString(),
                exception.Source
            );

        httpContext.Response.StatusCode = StatusCode;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCode,
            Type = ProblemDetailsUriHelper.GetProblemTypeUri(StatusCode),
            Title = Title,
            Detail = GetDetail(typedException),
        };

        var handled = await ProblemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails,
            }
        );

        if (handled)
            return true;
        
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
