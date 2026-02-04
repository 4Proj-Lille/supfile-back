namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public class UserController : BaseAuthController
{
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger,
        IUserService workspaceService,
        IUserRepository userRepository,
        IWebHostEnvironment env) : base(logger, userRepository, env)
    {
        _userService = workspaceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ApplicationUserModel>>> GetAll([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var users = await _userService.GetAllUsersAsync<ApplicationUserModel>(currentUser, pageNumber, pageSize);
        return ToActionResult(users);
    }

    // [HttpPatch("{userId:int}")]
    // public async Task<ActionResult<ApplicationUserModel>> Patch(int userId, [FromBody] UserPatchModel model)
    // {
    //     var currentUser = await GetAuthenticatedAppUserAsync();
    //     var entity = model.Adapt<ApplicationUser>();
    //
    //     var userResult = await _userService.UpdateAsync(userId, entity, currentUser);
    //     if (userResult.IsFailed)
    //     {
    //         return ToActionResult(Result.Fail(userResult.Errors));
    //     }
    //
    //     var userModel = userResult.Value.Adapt<ApplicationUserModel>();
    //     return ToActionResult(Result.Ok(userModel));
    // }

    // [HttpPatch("{userId:int}/Password")]
    // public async Task<ActionResult<ApplicationUserModel>> Patch(int userId,
    //     [FromBody] PasswordPatchModel model)
    // {
    //     var currentUser = await GetAuthenticatedAppUserAsync();
    //     var userResult =
    //         await _userService.UpdatePasswordAsync(userId, model.CurrentPassword, model.NewPassword,
    //             model.ConfirmNewPassword, currentUser);
    //     if (userResult.IsFailed)
    //     {
    //         return ToActionResult(Result.Fail(userResult.Errors));
    //     }
    //
    //     var userModel = userResult.Value.Adapt<ApplicationUserModel>();
    //     return ToActionResult(Result.Ok(userModel));
    // }

    // [HttpDelete("{userId:int}")]
    // public async Task<ActionResult> DeleteUser(int userId)
    // {
    //     var currentUser = await GetAuthenticatedAppUserAsync();
    //     var result = await _userService.DeleteUserAsync(currentUser, userId);
    //     return ToActionResult(result);
    // }
}