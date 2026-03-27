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
    
    [HttpPatch("{id:int}/SoftDelete")]
    public async Task<ActionResult<MediaModel>> SoftDelete(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var mediaResult = await _mediaService.GetByIdAsync<MediaModel>(id);
        if (mediaResult.IsFailed || mediaResult.Value == null)
        {
            return ToActionResult(Result.Fail(mediaResult.Errors));
        }

        var media = mediaResult.Value;
        
        if (media.OwnerId != currentUser.Id)
        {
            return ToActionResult(Result.Fail(new ForbiddenError("You are not authorized to delete this media.")));
        }
        
        media.IsActive = false;
        
        var updatedMediaResult = await _mediaService.UpdateAsync(id, media.Adapt<Media>(), currentUser);
        if (updatedMediaResult.IsFailed)
        {
            return ToActionResult(Result.Fail(updatedMediaResult.Errors));
        }

        var updatedMediaModel = updatedMediaResult.Value.Adapt<MediaModel>();
        return ToActionResult(Result.Ok(updatedMediaModel));
    }
    
    [HttpPatch("{id:int}/Restore")]
    public async Task<ActionResult<MediaModel>> Restore(int id)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var mediaResult = await _mediaService.GetByIdAsync<MediaModel>(id);
        if (mediaResult.IsFailed || mediaResult.Value == null)
        {
            return ToActionResult(Result.Fail(mediaResult.Errors));
        }

        var media = mediaResult.Value;
        
        if (media.OwnerId != currentUser.Id)
        {
            return ToActionResult(Result.Fail(new ForbiddenError("You are not authorized to restore this media.")));
        }
        
        media.IsActive = true;
        
        var updatedMediaResult = await _mediaService.UpdateAsync(id, media.Adapt<Media>(), currentUser);
        if (updatedMediaResult.IsFailed)
        {
            return ToActionResult(Result.Fail(updatedMediaResult.Errors));
        }

        var updatedMediaModel = updatedMediaResult.Value.Adapt<MediaModel>();
        return ToActionResult(Result.Ok(updatedMediaModel));
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
    
    [HttpGet("SoftDeleted")]
    public async Task<ActionResult<List<MediaModel>>> GetSoftDeleted()
    {        
        var currentUser = await GetAuthenticatedAppUserAsync();
        var medias = await _mediaService.GetSoftDeleted<MediaModel>(currentUser);
        
        return ToActionResult(medias);
    }
    
    [HttpDelete]
    public async Task<ActionResult<bool>> EmptyTrash()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var deletedResult = await _mediaService.DeleteAllSoftDeleted(currentUser);
        
        return ToActionResult(deletedResult);
    }

}