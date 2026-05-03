namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public class SettingsController : BaseAuthController
{
    private readonly ISettingService _settingsService;
    public SettingsController(ILogger<SettingsController> logger,
        IUserRepository userRepository, ISettingService settingsService,
        IWebHostEnvironment env) : base(logger, userRepository, env)
    {
        _settingsService = settingsService;
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<SettingDto>> Get()
    {
        var result = await _settingsService.GetSettingsAsync();
        return ToOkActionResult(result);
    }
}