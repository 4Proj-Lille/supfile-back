namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class FolderController : BaseAuthController
{
    private readonly IFolderService _folderService;

    public FolderController(
        ILogger<FolderController> logger,
        IFolderService folderService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userRepository, env)
    {
        _folderService = folderService;
    }
    
    [HttpPost]
    public async Task<ActionResult<FolderModel>> Post([FromBody] FolderPostModel model,
        [FromServices] IValidator<FolderPostModel> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, model);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        var currentUser = await GetAuthenticatedAppUserAsync();
        var createdFolderResult = await _folderService.AddOneAsync(currentUser, model.Adapt<Folder>());
        if (createdFolderResult.IsFailed)
        {
            return ToActionResult(Result.Fail(createdFolderResult.Errors));
        }

        var createdFolder = createdFolderResult.Value;
        
        var createdFolderModel = createdFolder.Adapt<FolderModel>();
        return ToActionResult(Result.Ok(createdFolderModel));
    }
    
    [HttpGet("Root")]
    public async Task<ActionResult<StorageModel>> GetFromRoot()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _folderService.GetFromRoot(currentUser);
    
        if (!result.IsSuccess)
            return ToActionResult(Result.Fail<StorageModel>(result.Errors));
    
        var (folders, medias) = result.Value;
    
        var storageModel = new StorageModel
        {
            Folders = folders.Adapt<List<FolderModel>>(),
            Medias = medias.Adapt<List<MediaModel>>()
        };
    
        return Ok(storageModel);
    }
}