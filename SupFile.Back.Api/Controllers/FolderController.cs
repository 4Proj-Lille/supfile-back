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

    [HttpGet("FromParent")]
    public async Task<ActionResult<StorageModel>> GetFromParent([FromQuery] int? id = null,
        [FromQuery] string? sort = "id")
    {
        var currentUser = await GetAuthenticatedAppUserAsync();

        var result = await _folderService.GetFromParent(currentUser, id, sort);

        return ToOkActionResult(result.Map(value =>
        {
            var (folders, medias) = value;
            return new StorageModel
            {
                Folders = folders.Adapt<List<FolderModel>>(), Medias = medias.Adapt<List<MediaModel>>()
            };
        }));
    }

    [HttpGet("Path")]
    public async Task<ActionResult<List<FolderModel>>> GetPath([FromQuery] int id)
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

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deletedResult = await _folderService.DeleteOneAsync(currentUser, id);

        return ToNoContentActionResult(deletedResult);
    }
}
