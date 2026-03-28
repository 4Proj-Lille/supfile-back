namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class MediaController : BaseAuthController
{
    private readonly IMediaService _mediaService;

    public MediaController(
        ILogger<MediaController> logger,
        IMediaService mediaService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userRepository, env)
    {
        _mediaService = mediaService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MediaModel>> Get(int id)
    {
        var medias = await _mediaService.GetByIdAsync<MediaModel>(id);

        return ToOkActionResult(medias);
    }

    [HttpPost]
    public async Task<ActionResult<MediaModel>> Post([FromBody] MediaPostModel model,
        [FromServices] IValidator<MediaPostModel> validator)
    {
        await validator.ValidateAndThrowAsync(model);

        var currentUser = await GetAuthenticatedAppUserAsync();
        var createdMediaResult = await _mediaService.AddOneAsync(currentUser, model.Adapt<Media>());
        var mediaModelResult = createdMediaResult.Map(m => m.Adapt<MediaModel>());
        return ToOkActionResult(mediaModelResult);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<MediaModel>> Patch(int id, [FromBody] MediaPatchModel model)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var entity = model.Adapt<Media>();
        var mediaResult = await _mediaService.UpdateAsync(id, entity, currentUser);

        var workspaceModel = mediaResult.Map(m => m.Adapt<MediaModel>());
        return ToOkActionResult(workspaceModel);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deletedResult = await _mediaService.DeleteOneAsync(currentUser, id);

        return ToNoContentActionResult(deletedResult);
    }

    [HttpPatch("{id:int}/SoftDelete")]
    public async Task<ActionResult<MediaModel>> SoftDelete(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var mediaResult = await _mediaService.GetByIdAsync<MediaModel>(id);
        if (mediaResult.IsFailed) return ToOkActionResult(mediaResult);

        var media = mediaResult.Value;

        if (media.OwnerId != currentUser.Id)
        {
            return ToErrorActionResult(Result.Fail(AuthErrors.UnauthorizedForEntity<Media, int>(id)));
        }

        media.IsActive = false;

        var updatedMediaResult = await _mediaService.UpdateAsync(id, media.Adapt<Media>(), currentUser);
        var mediaModelResult = updatedMediaResult.Map(m => m.Adapt<MediaModel>());
        return ToOkActionResult(mediaModelResult);
    }

    [HttpPatch("{id:int}/Restore")]
    public async Task<ActionResult<MediaModel>> Restore(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var mediaResult = await _mediaService.GetByIdAsync<MediaModel>(id);
        if (mediaResult.IsFailed) return ToOkActionResult(mediaResult);

        var media = mediaResult.Value;

        if (media.OwnerId != currentUser.Id)
        {
            return ToErrorActionResult(Result.Fail(AuthErrors.UnauthorizedForEntity<Media, int>(id)));
        }

        media.IsActive = true;

        var updatedMediaResult = await _mediaService.UpdateAsync(id, media.Adapt<Media>(), currentUser);
        var mediaModelResult = updatedMediaResult.Map(m => m.Adapt<MediaModel>());
        return ToOkActionResult(mediaModelResult);
    }

    [HttpGet("GlobalStorage")]
    public async Task<ActionResult<int>> GetGlobalStorage()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var storage = await _mediaService.GetGlobalStorage(currentUser);

        return ToOkActionResult(storage);
    }

    [HttpGet("StorageByExtension")]
    public async Task<ActionResult<Dictionary<string, int>>> GetStorageByExtension()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var storage = await _mediaService.GetStorageByExtension(currentUser);

        return ToOkActionResult(storage);
    }

    [HttpGet("SoftDeleted")]
    public async Task<ActionResult<List<MediaModel>>> GetSoftDeleted()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var medias = await _mediaService.GetSoftDeleted<MediaModel>(currentUser);

        return ToOkActionResult(medias);
    }

    [HttpDelete]
    public async Task<ActionResult<bool>> EmptyTrash()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deletedResult = await _mediaService.DeleteAllSoftDeleted(currentUser);

        return ToOkActionResult(deletedResult);
    }
}
