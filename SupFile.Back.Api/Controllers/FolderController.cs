namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class FolderController : BaseAuthController
{
    private readonly IFolderService _folderService;

    public FolderController(
        ILogger<FolderController> logger,
        UserManager<AuthIdentityUser> userManager,
        IFolderService folderService,
        IUserRepository userRepository,
        IWebHostEnvironment env
    ) : base(logger, userManager, userRepository, env)
    {
        _folderService = folderService;
    }
    
}