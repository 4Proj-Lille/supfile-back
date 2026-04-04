using SupFile.Back.Api.ExceptionHandlers.Base;

namespace SupFile.Back.Api.ExceptionHandlers;

internal sealed class GlobalExceptionHandler : BaseExceptionHandler<Exception>
{
    private readonly IHostEnvironment _hostEnvironment;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment hostEnvironment
    )
        : base(problemDetailsService, logger)
    {
        _hostEnvironment = hostEnvironment;
    }

    protected override int StatusCode => StatusCodes.Status500InternalServerError;
    protected override string Title => ErrorsRes.Internal_Server_Error_Title;
    protected override string Detail => ErrorsRes.Internal_Server_Error_Detail;

    protected override string GetDetail(Exception exception) =>
        _hostEnvironment.IsDevelopment()
            ? exception.Message
            : ErrorsRes.Internal_Server_Error_Detail;
}
