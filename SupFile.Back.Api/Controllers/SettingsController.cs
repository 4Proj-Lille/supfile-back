namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public class SettingsController : BaseAuthController
{
    public SettingsController(ILogger<SettingsController> logger,
        IUserRepository userRepository,
        IWebHostEnvironment env) : base(logger, userRepository, env)
    {
    }
    
    // [HttpGet]
    // public async Task<ActionResult<SettingDto>> Get()
    // {
    //     var settings = new SettingDto
    //     {
    //         AllocatedSpace = _environment.
    //     };        
    //     
    //     return ToOkActionResult(Result.Ok(settings));
    // }
}