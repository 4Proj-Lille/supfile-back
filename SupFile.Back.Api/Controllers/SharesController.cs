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
    
    [HttpGet]
    public async Task<ActionResult<StorageModel>> GetShares([FromQuery] SearchQuery query, [FromQuery] int? folderId = null)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var sharesResult = await _shareService.GetAllAsync(currentUser, query, folderId);
        return ToOkActionResult(sharesResult.Map(value =>
        {
            var (folders, medias) = value;
            return new StorageModel
            {
                Folders = folders.Adapt<List<FolderModel>>(), Medias = medias.Adapt<List<MediaModel>>()
            };
        }));    }
    
}