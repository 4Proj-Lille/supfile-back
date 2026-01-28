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
}