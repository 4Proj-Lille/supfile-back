namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class FoldersController : BaseAuthController
{
    private readonly IFolderService _folderService;

    public FoldersController(
        ILogger<FoldersController> logger,
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
        await validator.ValidateAndThrowAsync(model);

        var currentUser = await GetAuthenticatedAppUserAsync();
        var createdFolderResult = await _folderService.AddOneAsync(currentUser, model.Adapt<Folder>());

        var modelResult = createdFolderResult.Map(m => m.Adapt<FolderModel>());
        return ToCreatedAtActionResult(modelResult, nameof(GetById));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FolderModel>> GetById(int id)
    {
        var folders = await _folderService.GetByIdAsync<FolderModel>(id);

        return ToOkActionResult(folders);
    }

    [HttpGet("FolderContents")]
    public async Task<ActionResult<StorageModel>> GetFolderContents([FromQuery] SearchQuery query, [FromQuery] int? folderId = null)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();

        var result = await _folderService.GetFolderContents(currentUser, folderId, query);

        return ToOkActionResult(result.Map(value =>
        {
            var (folders, medias) = value;
            return new StorageModel
            {
                Folders = folders.Adapt<List<FolderModel>>(), Medias = medias.Adapt<List<MediaModel>>()
            };
        }));
    }

    [HttpGet("{id:int}/Path")]
    public async Task<ActionResult<List<FolderModel>>> GetPath(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();

        var result = await _folderService.GetPath(currentUser, id);

        var folderModelResult = result.Map(m => m.Adapt<List<FolderModel>>());
        return ToOkActionResult(folderModelResult);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<FolderModel>> Patch(int id, [FromBody] FolderPatchModel model)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var entity = model.Adapt<Folder>();
        var folderResult = await _folderService.UpdateAsync(id, entity, currentUser);

        var folderModelResult = folderResult.Map(m => m.Adapt<FolderModel>());
        return ToOkActionResult(folderModelResult);
    }

    [HttpDelete("{id:int}/SoftDelete")]
    public async Task<ActionResult<FolderModel>> SoftDelete(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deletedResult = await _folderService.SoftDeleteAsync(currentUser, id);
        var folderModelResult = deletedResult.Map(m => m.Adapt<FolderModel>());
        return ToOkActionResult(folderModelResult);
    }
    
    [HttpGet("{folderId:int}/Download")]
    public async Task<IActionResult> DownloadFolder(int folderId)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _folderService.DownloadFolderAsync(folderId, currentUser);
        if (result.IsFailed)
        {
            return ToErrorActionResult(result.ToResult());
        }
        
        var folderName = result.Value.Item1;
        var content = result.Value.Item2;

        return File(content, "application/zip", $"{folderName}.zip");
    }
    
    [HttpGet("TotalSize")]
    public async Task<ActionResult<long>> GetFolderSizeRecursive([FromQuery] int? folderId = null) 
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _folderService.GetFolderSizeRecursive(folderId, currentUser);
        return ToOkActionResult(result);
    }
}
