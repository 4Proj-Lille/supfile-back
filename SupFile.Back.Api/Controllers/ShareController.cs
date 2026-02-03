namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class ShareController : BaseAuthController
{
    private readonly IShareService _shareService;

    public ShareController(
        ILogger<ShareController> logger,
        IShareService shareService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userRepository, env)
    {
        _shareService = shareService;
    }
    
}