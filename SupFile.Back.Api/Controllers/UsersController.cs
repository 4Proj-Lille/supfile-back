namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public class UsersController : BaseAuthController
{
    private readonly IUserService _userService;

    public UsersController(ILogger<UsersController> logger,
        IUserService workspaceService,
        IUserRepository userRepository,
        IWebHostEnvironment env) : base(logger, userRepository, env)
    {
        _userService = workspaceService;
    }

    // [HttpGet]
    // public async Task<ActionResult<List<ApplicationUserModel>>> GetAll([FromQuery] int pageNumber = 1,
    //     [FromQuery] int pageSize = 10)
    // {
    //     var currentUser = await GetAuthenticatedAppUserAsync();
    //     var users = await _userService.GetAllUsersAsync<ApplicationUserModel>(currentUser, pageNumber, pageSize);
    //     return ToOkActionResult(users);
    // }
    
    [HttpGet("{userId:int}")]
    public async Task<ActionResult<ApplicationUserModel>> GetUserById(int userId)
    {
        var user = await _userService.GetByIdAsync<ApplicationUserModel>(userId);
        return ToOkActionResult(user);
    }
    
    [HttpGet("ByName")]
    public async Task<ActionResult<List<ApplicationUserModel>>> GetUserByName([FromQuery] string name)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var users = await _userService.GetUsersByNameAsync<ApplicationUserModel>(currentUser, name);
        return ToOkActionResult(users);
    }
    
    [HttpPatch("{userId:int}")]
    public async Task<ActionResult<UserModel>> Patch(int userId, [FromBody] UserPatchModel model,
        [FromServices] IValidator<UserPatchModel> validator) {
         var validationResult = await validator.ValidateAsync(model);
         if (!validationResult.IsValid)
         {
             validationResult.AddToModelState(ModelState);
             return ValidationProblem(ModelState);
         }
         var currentUser = await GetAuthenticatedAppUserAsync();
         var entity = model.Adapt<ApplicationUser>();
    
         var userResult = await _userService.UpdateAsync(userId, entity, currentUser);
         if (userResult.IsFailed)
         {
             return ToErrorActionResult(userResult.ToResult());
         }
         var userModel = userResult.Value.Adapt<UserModel>();
         return ToOkActionResult(Result.Ok(userModel));
    }

    [HttpPatch("{userId:int}/Password")]
    public async Task<ActionResult<ApplicationUserModel>> Patch(int userId,
         [FromBody] PasswordPatchModel model)
    { 
        var currentUser = await GetAuthenticatedAppUserAsync();
        var userResult = await _userService.UpdatePasswordAsync(userId, model.CurrentPassword, model.NewPassword, model.ConfirmNewPassword, currentUser);
        if (userResult.IsFailed) 
        {
            return ToErrorActionResult(userResult.ToResult());
        }
        
        var userModel = userResult.Value.Adapt<ApplicationUserModel>(); 
        return ToOkActionResult(Result.Ok(userModel));
    }

    [HttpDelete("{userId:int}")]
    public async Task<ActionResult> DeleteUser(int userId)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _userService.DeleteUserAsync(currentUser, userId);
        return ToNoContentActionResult(result);
    }
    
    [AllowAnonymous]
    [HttpGet("{userId:int}/ProfilePicture")]
    public async Task<IActionResult> PreviewPicture(int userId)
    {
        var mediaFile = await _userService.DownloadPicture(userId);

        if (mediaFile.IsFailed)
            return NotFound();

        var file = mediaFile.Value.Item1;
        var contentType = mediaFile.Value.Item2;
        var fileName = mediaFile.Value.Item3;

        Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");

        return File(file, contentType);
    }
    
    [HttpPatch("{userId:int}/ProfilePicture")]
    public async Task<ActionResult<ApplicationUserModel>> UpdateProfilePicture(int userId, IFormFile? file = null)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();

        var createdMediaResult = await _userService.UpdateProfilePicture(currentUser, file, userId);
        var mediaModelResult = createdMediaResult.Map(m => m.Adapt<ApplicationUserModel>());
        return ToOkActionResult(mediaModelResult);
    }

}