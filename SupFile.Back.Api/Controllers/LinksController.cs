using SupFile.Back.Core.Enums;

namespace SupFile.Back.Api.Controllers;

[Route("api/[controller]")]
public sealed class LinksController : BaseAuthController
{
    private readonly ILinkService _linkService;

    public LinksController(
        ILogger<LinksController> logger,
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

        return ToOkActionResult(inviteLinkResult);
    }

    [HttpPost("generate/{userId:int}/email")]
    public async Task<ActionResult<string>> GenerateEmailInviteLink(
        int userId,
        [FromQuery] int itemId,
        [FromQuery] InvitationItemType itemType)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();

        var inviteLinkResult = await _linkService.GenerateEmailShareLinkAsync(currentUser, itemId, itemType, userId);

        return ToOkActionResult(inviteLinkResult);
    }

    [HttpGet("pending")]
    public async Task<ActionResult<List<PendingInvitationModel>>> GetPendingInvitations()
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _linkService.GetPendingInvitationsAsync(currentUser);
        return ToOkActionResult(result.Map(links => links.Adapt<List<PendingInvitationModel>>()));
    }

    [HttpPost("accept")]
    public async Task<ActionResult> AcceptEmailInviteLink([FromQuery] string token)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();

        var inviteLinkResult = await _linkService.AcceptShareLinkAsync(currentUser, token);
        return ToNoContentActionResult(inviteLinkResult.ToResult());
    }
}
