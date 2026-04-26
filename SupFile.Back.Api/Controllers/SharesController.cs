using SupFile.Back.Core.Enums;

namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class SharesController : BaseAuthController
{
    private readonly IShareService _shareService;

    public SharesController(
        ILogger<SharesController> logger,
        IShareService shareService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userRepository, env)
    {
        _shareService = shareService;
    }
    
    [HttpPatch("{objectId:int}/Access")]
    public async Task<ActionResult<List<ApplicationUserModel>>> UpdateSharePermissions(int objectId, [FromQuery] ObjectType type)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var updateResult = await _shareService.GetAccessUsersAsync<ApplicationUserModel>(objectId, currentUser, type);
        return ToOkActionResult(updateResult);
    }
    
}