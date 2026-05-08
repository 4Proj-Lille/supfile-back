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
    
    [HttpGet("{objectId:int}/Access")]
    public async Task<ActionResult<List<ApplicationUserModel>>> GetAccessUsersAsync(int objectId, [FromQuery] ObjectType type)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var updateResult = await _shareService.GetAccessUsersAsync<ApplicationUserModel>(objectId, currentUser, type);
        return ToOkActionResult(updateResult);
    }
    
    [HttpPatch("Permission")]
    public async Task<ActionResult<ShareModel>> UpdatePermission([FromBody] UpdateSharePermissionDto dto)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _shareService.UpdatePermissionAsync(currentUser, dto);
        var shareModelResult = result.Map(share => share.Adapt<ShareModel>());
        return ToOkActionResult(shareModelResult);
    }

    [HttpDelete("Access")]
    public async Task<ActionResult> RevokeAccess([FromQuery] int objectId, [FromQuery] int userId, [FromQuery] InvitationItemType type)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _shareService.RevokeAccessAsync(currentUser, objectId, userId, type);
        return ToNoContentActionResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<StorageModel>> GetShares([FromQuery] SearchQuery query, [FromQuery] int? folderId = null, [FromQuery] int? limit = null)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var sharesResult = await _shareService.GetAllAsync<FolderModel, MediaModel>(currentUser, query, folderId, limit);
        return ToOkActionResult(sharesResult.Map(value =>
        {
            var (folders, medias) = value;
            return new StorageModel
            {
                Folders = folders, Medias = medias
            };
        }));    }
    
}