namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class ShareController : BaseAuthController
{
    private readonly IShareService _shareService;

    public ShareController(
        ILogger<ShareController> logger,
        UserManager<AuthIdentityUser> userManager,
        IShareService shareService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userManager, userRepository, env)
    {
        _shareService = shareService;
    }
    
}