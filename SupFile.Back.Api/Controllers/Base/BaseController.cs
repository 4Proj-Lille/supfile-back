using SupFile.Back.Api.Helpers;
using SupFile.Back.Core.Errors.Base;

namespace SupFile.Back.Api.Controllers.Base;

public class BaseController : ControllerBase
{
    private readonly IHostEnvironment _environment;

    public BaseController(ILogger<BaseController> logger, IHostEnvironment environment)
    {
        Logger = logger;
        _environment = environment;
    }

    protected ILogger Logger { get; }

    protected ActionResult<T> ToOkActionResult<T>(Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
            return Ok(result.Value);

        return ToErrorActionResult(result);
    }

    protected ActionResult ToNoContentActionResult(Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess) return NoContent();

        return ToErrorActionResult(result);
    }

    protected ActionResult<T> ToCreatedAtActionResult<T>(Result<T> result, string? actionName)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
            return CreatedAtAction(actionName, new { id = result.Value }, result.Value);

        return ToErrorActionResult(result);
    }
    
    protected ActionResult ToCreatedAtActionResult(Result result, string? actionName, object? routeValues = null)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
            return CreatedAtAction(actionName, routeValues, null);

        return ToErrorActionResult(result);
    }

    private ActionResult ToErrorActionResult<T>(Result<T> result)
        => ToErrorActionResult(result.ToResult());

    protected ActionResult ToErrorActionResult(Result result)
    {
        var customErrors = result.Errors.OfType<CustomError>().ToList();

        if (customErrors.Any(x => x.ErrorType == ErrorType.BadRequest))
        {
            foreach (var err in result.Errors)
            {
                if (err is CustomError customErr)
                    ModelState.AddModelError(customErr.Title, customErr.Message);
                else
                    ModelState.AddModelError(nameof(ErrorType.BadRequest), err.Message);
            }

            var firstBadRequestError = customErrors.First(x => x.ErrorType == ErrorType.BadRequest);
            var status = ProblemDetailsHelper.GetStatus(firstBadRequestError);
            var validationProblemDetails = new ValidationProblemDetails(ModelState)
            {
                Status = status,
                Type = ProblemDetailsUriHelper.GetProblemTypeUri(status),
                Title = ErrorsRes.BadRequest_Title
            };

            return BadRequest(validationProblemDetails);
        }

        var error = customErrors.Count > 0 ? customErrors[0] : null;

        if (error is null)
            return StatusCode(StatusCodes.Status500InternalServerError);

        var problemDetails = ProblemDetailsHelper.ProblemDetailsBuilder(
            error,
            HttpContext?.TraceIdentifier
        );

        return error.ErrorType switch
        {
            ErrorType.NotFound => NotFound(problemDetails),
            ErrorType.BadRequest => BadRequest(problemDetails),
            ErrorType.Forbidden => StatusCode(problemDetails.Status ?? 403, problemDetails),
            ErrorType.Failure => StatusCode(problemDetails.Status ?? 500, problemDetails),
            _ => StatusCode(problemDetails.Status ?? 500, problemDetails),
        };
    }
}
