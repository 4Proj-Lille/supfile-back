using SupFile.Back.Core.Enums;

namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]

public class BinsController: BaseAuthController
{
    private readonly IBinService _binService;

    public BinsController(
        ILogger<BinsController> logger,
        IBinService binService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userRepository, env)
    {
        _binService = binService;
    }
    
    [HttpPatch("{id:int}/Restore")]
    public async Task<ActionResult> Restore(int id, [FromQuery] ObjectType type)
    {        
        var currentUser = await GetAuthenticatedAppUserAsync();
        var restoreResult = await _binService.RestoreAsync(id, currentUser, type);
        return ToNoContentActionResult(restoreResult); 
        
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteOneAsync(int id, [FromQuery] ObjectType type)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deleteResult = await _binService.DeleteOneAsync(id , currentUser, type);
        return ToNoContentActionResult(deleteResult);
    }
    
    [HttpDelete]
    public async Task<ActionResult> EmptyBin()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deleteResult = await _binService.EmptyBinAsync(currentUser);
        return ToNoContentActionResult(deleteResult);
    }
    
    [HttpGet]
    public async Task<ActionResult<StorageModel>> GetBinItems([FromQuery] ObjectType? type)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var binItemsResult = await _binService.GetBinItemsAsync(currentUser, type);
        return ToOkActionResult(binItemsResult.Map(value =>
        {
            var (folders, medias) = value;
            return new StorageModel
            {
                Folders = folders.Adapt<List<FolderModel>>(), Medias = medias.Adapt<List<MediaModel>>()
            };
        }));
    }
}
