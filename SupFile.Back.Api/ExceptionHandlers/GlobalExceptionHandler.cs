// GlobalExceptionHandler.cs
using SupFile.Back.Api.ExceptionHandlers.Base;

namespace SupFile.Back.Api.ExceptionHandlers;

internal sealed class GlobalExceptionHandler : BaseExceptionHandler<Exception>
{
    private readonly IHostEnvironment _hostEnvironment;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment hostEnvironment)
        : base(problemDetailsService, logger)
    {
        _hostEnvironment = hostEnvironment;
    }

    protected override int StatusCode => StatusCodes.Status500InternalServerError;
    protected override string Title => ErrorsRes.Internal_Server_Error_Title;

    protected override string GetDetail(Exception exception)
    {
        if (!_hostEnvironment.IsDevelopment())
        {
            return ErrorsRes.Internal_Server_Error_Detail;
        }

        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(exception.Message))
        {
            parts.Add($"Message: {exception.Message}");
        }

        if (exception.InnerException is not null)
        {
            parts.Add($"InnerException: {exception.InnerException}");
        }

        return parts.Count > 0
            ? string.Join(" | ", parts)
            : ErrorsRes.Internal_Server_Error_Detail;
    }
}
