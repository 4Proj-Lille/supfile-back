namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class MediasController : BaseAuthController
{
    private readonly IMediaService _mediaService;

    public MediasController(
        ILogger<MediasController> logger,
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
        var media = await _mediaService.GetByIdAsync<MediaModel>(id);

        return ToOkActionResult(media);
    }

    [HttpGet("{id:int}/Download")]
    public async Task<IActionResult> DownloadPicture(int id)
    {
        var mediaResult = await _mediaService.GetByIdAsync<MediaModel>(id);
        if (mediaResult.IsFailed) return ToErrorActionResult(mediaResult.ToResult());

        var mediaFile = await _mediaService.DownloadPicture(mediaResult.Value.Name, mediaResult.Value.Extension);
        if (mediaFile.IsFailed) return ToErrorActionResult(mediaFile.ToResult());

        var file = mediaFile.Value.Item1;
        var contentType = mediaFile.Value.Item2;

        return File(file, contentType, mediaResult.Value.Name);
    }

    [HttpPost]
    public async Task<ActionResult<MediaModel>> Post(IFormFile file, [FromQuery] int? folderId)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();

        var createdMediaResult = await _mediaService.AddOneAsync(currentUser, file, folderId);
        var mediaModelResult = createdMediaResult.Map(m => m.Adapt<MediaModel>());
        return ToOkActionResult(mediaModelResult);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<MediaModel>> Patch(int id, [FromBody] MediaPatchModel model)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var entity = model.Adapt<Media>();
        var mediaResult = await _mediaService.UpdateAsync(id, entity, currentUser);

        var mediaModelResult = mediaResult.Map(m => m.Adapt<MediaModel>());
        return ToOkActionResult(mediaModelResult);
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
        if (mediaResult.IsFailed) return ToErrorActionResult(mediaResult.ToResult());

        var media = mediaResult.Value;

        if (media.OwnerId != currentUser.Id)
        {
            return ToErrorActionResult(AuthErrors.UnauthorizedForEntity<Media, int>(id));
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
        if (mediaResult.IsFailed) return ToErrorActionResult(Result.Fail(mediaResult.Errors));

        var media = mediaResult.Value;

        if (media.OwnerId != currentUser.Id)
        {
            return ToErrorActionResult(AuthErrors.UnauthorizedForEntity<Media, int>(id));
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
    public async Task<ActionResult> EmptyTrash()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deletedResult = await _mediaService.DeleteAllSoftDeleted(currentUser);

        return ToNoContentActionResult(deletedResult.ToResult());
    }
}
