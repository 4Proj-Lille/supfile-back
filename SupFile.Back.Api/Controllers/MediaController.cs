namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class MediaController : BaseAuthController
{
    private readonly IMediaService _mediaService;

    public MediaController(
        ILogger<MediaController> logger,
        UserManager<AuthIdentityUser> userManager,
        IMediaService mediaService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userManager, userRepository, env)
    {
        _mediaService = mediaService;
    }
    
}