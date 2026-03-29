using SupFile.Back.Api.ExceptionHandlers.Base;

namespace SupFile.Back.Api.ExceptionHandlers;

internal sealed class GlobalExceptionHandler : BaseExceptionHandler<Exception>
{
    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger
    )
        : base(problemDetailsService, logger) { }

    protected override int StatusCode => StatusCodes.Status500InternalServerError;
    protected override string Title => ErrorsRes.Internal_Server_Error_Title;
    protected override string Detail => ErrorsRes.Internal_Server_Error_Detail;
}
