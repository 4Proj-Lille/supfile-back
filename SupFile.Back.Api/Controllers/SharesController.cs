namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class SharesController : BaseAuthController
{
    private readonly IShareService _shareService;

    public SharesController(
        ILogger<SharesController> logger,
        IShareService shareService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userRepository, env)
    {
        _shareService = shareService;
    }
    
}