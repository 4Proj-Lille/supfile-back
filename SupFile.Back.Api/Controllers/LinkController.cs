namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class LinkController : BaseAuthController
{
    private readonly ILinkService _linkService;

    public LinkController(
        ILogger<LinkController> logger,
        ILinkService linkService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userRepository, env)
    {
        _linkService = linkService;
    }
    
}