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

        return ToActionResult(medias);
    }
    
    [HttpPost]
    public async Task<ActionResult<MediaModel>> Post([FromBody] MediaPostModel model,
        [FromServices] IValidator<MediaPostModel> validator)
    {
        var validationCheck = await ValidateAndToActionResult(validator, model);
        if (validationCheck is not null)
        {
            return validationCheck;
        }

        var currentUser = await GetAuthenticatedAppUserAsync();
        var createdMediaResult = await _mediaService.AddOneAsync(currentUser, model.Adapt<Media>());
        if (createdMediaResult.IsFailed)
        {
            return ToActionResult(Result.Fail(createdMediaResult.Errors));
        }

        var createdMedia = createdMediaResult.Value;
        
        var createdMediaModel = createdMedia.Adapt<MediaModel>();
        return ToActionResult(Result.Ok(createdMediaModel));
    }
    
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<MediaModel>> Patch(int id, [FromBody] MediaPatchModel model)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var entity = model.Adapt<Media>();
        var mediaResult = await _mediaService.UpdateAsync(id, entity, currentUser);
        if (mediaResult.IsFailed)
        {
            return ToActionResult(Result.Fail(mediaResult.Errors));
        }

        var workspaceModel = mediaResult.Value.Adapt<MediaModel>();
        return ToActionResult(Result.Ok(workspaceModel));
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deletedResult = await _mediaService.DeleteOneAsync<MediaModel>(currentUser, id);

        return ToActionResult(deletedResult);
    }
    
    [HttpGet("GlobalStorage")]
    public async Task<ActionResult<int>> GetGlobalStorage()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var storage = await _mediaService.GetGlobalStorage(currentUser);

        return ToActionResult(storage);
    }
    
    [HttpGet("StorageByExtension")]
    public async Task<ActionResult<Dictionary<string, int>>> GetStorageByExtension()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var storage = await _mediaService.GetStorageByExtension(currentUser);

        return ToActionResult(storage);
    }
}