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

    // protected ActionResult<T> ToActionResult<T>(Result<T> result)
    // {
    //     if (result.IsSuccess)
    //     {
    //         if (result.Value == null)
    //         {
    //             return NoContent();
    //         }
    //
    //         return Ok(result.Value);
    //     }
    //
    //     var firstError = result.Errors.First();
    //     logger.LogWarning("Handled error result: {ErrorType} - {Message}", firstError.GetType().Name,
    //         firstError.Message);
    //
    //
    //     return firstError switch
    //     {
    //         BadRequestError e => new BadRequestObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
    //         UnauthorizedError e => new UnauthorizedObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
    //         ConflictError e => new ConflictObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
    //         UnprocessableEntityError e => new UnprocessableEntityObjectResult(
    //             ProblemBuilder.Create(e.StatusCode, e.Message)),
    //         NotFoundError e => new NotFoundObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
    //         CustomError e => new ObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
    //         _ => new ObjectResult(ProblemBuilder.Create(500,
    //             environment.IsDevelopment() ? firstError.Message : "Internal server error"))
    //     };
    // }

    protected ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        if (!result.IsSuccess) return HandleError(result.Errors);
        
        if (result.Value == null)
        {
            return NoContent();
        }

        return Ok(result.Value);

    }

    protected ActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleError(result.Errors);
    }


    private ObjectResult HandleError(List<IError> errors)
    {
        var firstError = errors.First();
        Logger.LogWarning("Handled error result: {ErrorType} - {Message}", firstError.GetType().Name,
            firstError.Message);

        return firstError switch
        {
            BadRequestError e => new BadRequestObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
            UnauthorizedError e => new UnauthorizedObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
            ConflictError e => new ConflictObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
            UnprocessableEntityError e => new UnprocessableEntityObjectResult(
                ProblemBuilder.Create(e.StatusCode, e.Message)),
            NotFoundError e => new NotFoundObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
            CustomError e => new ObjectResult(ProblemBuilder.Create(e.StatusCode, e.Message)),
            _ => new ObjectResult(ProblemBuilder.Create(HttpStatusCode.InternalServerError,
                _environment.IsDevelopment() ? firstError.Message : "Internal server error"))
        };
    }

    protected async Task<ActionResult?> ValidateAndToActionResult<TModel>(
        IValidator<TModel> validator,
        TModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (validationResult.IsValid)
        {
            return null;
        }

        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        var result = Result.Fail(new BadRequestError(string.Join(", ", errors)));
        return ToActionResult(result);
    }
}
