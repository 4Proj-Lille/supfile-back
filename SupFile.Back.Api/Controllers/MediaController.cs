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
    
}