namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class LinkController : BaseAuthController
{
    private readonly ILinkService _linkService;

    public LinkController(
        ILogger<LinkController> logger,
        UserManager<AuthIdentityUser> userManager,
        ILinkService linkService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userManager, userRepository, env)
    {
        _linkService = linkService;
    }
    
}