namespace SupFile.Back.Business.Services;

public class MediaService : BaseService<Media, int, IMediaRepository>, IMediaService
{
    private readonly IUserService _userService;

    public MediaService(ILogger<MediaService> logger, IMediaRepository repository,
        IUserService userService
    ) : base(logger,
        repository)
    {
        _userService = userService;
    }
    
    public async Task<Result<Media>> AddOneAsync(ApplicationUser currentUser, Media entity)
    {
        entity.OwnerId = currentUser.Id;
        entity.CreatedDate = DateTime.Now;
        var addFolder = await AddAsync(entity);

        return addFolder;
    }
    
    
    public async Task<Result<bool>> DeleteOneAsync<TMapped>(ApplicationUser currentUser, int id)
    {
        var mediaResult = await Repository.GetByIdAsync<TMapped>(id);
        if (mediaResult.IsFailed || mediaResult.Value == null)
        {
            return Result.Fail(mediaResult.Errors);
        }

        var media = mediaResult.Value;

        if (media.Adapt<Media>().OwnerId == currentUser.Id)
        {
            return await DeleteAsync<TMapped>(id);
        }

        return Result.Fail(new ForbiddenError("You are not authorized to delete this media."));
    }
    
    public async Task<Result<List<Media>>> GetFrom(ApplicationUser currentUser, int? id){
        var mediaResult = await Repository.GetFrom<Media>(currentUser, id);

        return mediaResult;
    }
    
    public async Task<Result<Media>> UpdateAsync(int id, Media entity, ApplicationUser currentUser)
    {
        var MediaResult = await Repository.GetByIdAsync<Media>(id);
        if (MediaResult.IsFailed || MediaResult.Value == null)
        {
            return MediaResult;
        }

        var media = MediaResult.Value;

        if ( media.OwnerId != currentUser.Id)
        {
            return Result.Fail(new ForbiddenError("You are not authorized to update this media."));
        }

        foreach (var prop in typeof(Media).GetProperties())
        {
            var value = prop.GetValue(entity);

            if (!ScalarTypeHelper.IsScalarProperty(prop))
            {
                continue;
            }

            if (value != null)
            {
                prop.SetValue(media, value);
            }
        }

        return await UpdateAsync(id, media);
    }
    
}