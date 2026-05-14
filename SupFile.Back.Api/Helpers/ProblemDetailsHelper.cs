using SupFile.Back.Core.Errors.Base;

namespace SupFile.Back.Api.Helpers;

public static class ProblemDetailsHelper
{
    public static ProblemDetails ProblemDetailsBuilder(CustomError error, string? traceId)
    {
        ArgumentNullException.ThrowIfNull(error);

        var problemDetails = new ProblemDetails
        {
            Title = error.Title,
            Detail = error.Message,
            Status = GetStatus(error)
        };

        if (problemDetails.Status is not null)
            problemDetails.Type = ProblemDetailsUriHelper.GetProblemTypeUri(
                (int)problemDetails.Status
            );

        if (!string.IsNullOrEmpty(traceId))
            problemDetails.Extensions[nameof(traceId)] = traceId;

        return problemDetails;
    }

    public static int GetStatus(CustomError error)
    {
        return error.ErrorType switch
        {
            ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };
    }
}
