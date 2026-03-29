using SupFile.Back.Core.Enums;

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
    
    [HttpPost("generate")]
    public async Task<ActionResult<string>> GenerateInviteLink(
        [FromQuery] int itemId, 
        [FromQuery] InvitationItemType itemType)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
    
        var inviteLinkResult = itemType switch
        {
            InvitationItemType.Media => await _linkService.GenerateMediaShareLinkAsync(currentUser, itemId),
            InvitationItemType.Folder => await _linkService.GenerateFolderShareLinkAsync(currentUser, itemId),
            _ => Result.Fail("Invalid InvitationItemType")
        };

        if (inviteLinkResult.IsFailed)
        {
            return ToActionResult(Result.Fail(inviteLinkResult.Errors));
        }

        return Ok(inviteLinkResult.Value);
    }
    
    [HttpPost("generate/{userId:int}/email")]
    public async Task<ActionResult<string>> GenerateEmailInviteLink(
        int userId,
        [FromQuery] int itemId, 
        [FromQuery] InvitationItemType itemType)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        
        var inviteLinkResult = await _linkService.GenerateEmailShareLinkAsync(currentUser, itemId, itemType.ToString(), userId);
        if (inviteLinkResult.IsFailed)
        {
            return ToActionResult(Result.Fail(inviteLinkResult.Errors));
        }
 
        return Ok(inviteLinkResult.Value);
    }
    
    [HttpPost("accept")]
    public async Task<ActionResult<ShareModel>> AcceptEmailInviteLink([FromQuery] string token)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        
        var inviteLinkResult = await _linkService.AcceptShareLinkAsync(currentUser, token);
        if (inviteLinkResult.IsFailed)
        {
            return ToActionResult(Result.Fail(inviteLinkResult.Errors));
        }

        return Ok(inviteLinkResult.Value.Adapt<ShareModel>());
    }
    
    
}