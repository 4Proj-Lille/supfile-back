using SupFile.Back.Core.Enums;

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

    [HttpGet("{mediaUniqueId:Guid}/Download")]
    public async Task<IActionResult> DownloadPicture(Guid mediaUniqueId)
    {
        var mediaFile = await _mediaService.DownloadPicture(mediaUniqueId);

        var file = mediaFile.Value.Item1;
        var contentType = mediaFile.Value.Item2;
        var fileName = mediaFile.Value.Item3;

        return File(file, contentType, fileName);
    }

    [AllowAnonymous]
    [HttpGet("{mediaUniqueId:Guid}/Preview")]
    public async Task<IActionResult> PreviewPicture(Guid mediaUniqueId)
    {
        var mediaFile = await _mediaService.DownloadPicture(mediaUniqueId);

        if (mediaFile.IsFailed)
            return NotFound();

        var file = mediaFile.Value.Item1;
        var contentType = mediaFile.Value.Item2;
        var fileName = mediaFile.Value.Item3;

        Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");

        return File(file, contentType);
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

    [HttpDelete("{id:int}/SoftDelete")]
    public async Task<ActionResult<MediaModel>> SoftDelete(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deletedResult = await _mediaService.SoftDeleteAsync(currentUser, id);
        var mediaModelResult = deletedResult.Map(m => m.Adapt<MediaModel>());
        return ToOkActionResult(mediaModelResult);
    }

    [HttpGet("StorageSize")]
    public async Task<ActionResult<Dictionary<string, int>>> GetStorageSizeByExtension(StorageSizeGroupBy groupBy)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var storage = await _mediaService.GetStorageSize(currentUser, groupBy);

        return ToOkActionResult(storage);
    }
}
