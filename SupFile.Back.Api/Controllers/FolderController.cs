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
    
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FolderModel>> Get(int id)
    {
        var workspaces = await _folderService.GetByIdAsync<FolderModel>(id);

        return ToActionResult(workspaces);
    }
    
    [HttpGet("FromParent")]
    public async Task<ActionResult<StorageModel>> GetFromParent([FromQuery] int? id = null)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _folderService.GetFromParent(currentUser, id);
    
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
    
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<FolderModel>> Patch(int id, [FromBody] FolderPatchModel model)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var entity = model.Adapt<Folder>();
        var workspaceResult = await _folderService.UpdateAsync(id, entity, currentUser);
        if (workspaceResult.IsFailed)
        {
            return ToActionResult(Result.Fail(workspaceResult.Errors));
        }

        var workspaceModel = workspaceResult.Value.Adapt<FolderModel>();
        return ToActionResult(Result.Ok(workspaceModel));
    }
}