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
    public async Task<ActionResult<List<ApplicationUserModel>>> GetAccessUsersAsync(int objectId, [FromQuery] ObjectType type)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var updateResult = await _shareService.GetAccessUsersAsync<ApplicationUserModel>(objectId, currentUser, type);
        return ToOkActionResult(updateResult);
    }
    
    [HttpPatch("Permission")]
    public async Task<ActionResult<Share>> UpdatePermission([FromBody] UpdateSharePermissionDto dto)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _shareService.UpdatePermissionAsync(currentUser, dto);
        return ToOkActionResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<StorageModel>> GetShares([FromQuery] SearchQuery query, [FromQuery] int? folderId = null)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var sharesResult = await _shareService.GetAllAsync<FolderModel, MediaModel>(currentUser, query, folderId);
        return ToOkActionResult(sharesResult.Map(value =>
        {
            var (folders, medias) = value;
            return new StorageModel
            {
                Folders = folders, Medias = medias
            };
        }));    }
    
}