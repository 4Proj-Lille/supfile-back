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
        var result = await _linkService.GetPendingInvitationsAsync<PendingInvitationModel>(currentUser);
        return ToOkActionResult(result);
    }

    [HttpDelete("decline")]
    public async Task<ActionResult> DeclineInviteLink([FromQuery] string token)
    {
        var currentUser = await GetAuthenticatedAppUserAsync();
        var result = await _linkService.DeclineShareLinkAsync(currentUser, token);
        return ToNoContentActionResult(result);
    }

    [AllowAnonymous]
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptEmailInviteLink([FromQuery] string token)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            var downloadResult = await _linkService.DownloadByTokenAsync(token);
            if (downloadResult.IsFailed)
                return ToErrorActionResult(downloadResult.ToResult());

            var (bytes, contentType, fileName) = downloadResult.Value;
            return File(bytes, contentType, fileName);
        }

        var currentUser = await GetAuthenticatedAppUserAsync();
        var inviteLinkResult = await _linkService.AcceptShareLinkAsync(currentUser, token);
        return ToNoContentActionResult(inviteLinkResult.ToResult());
    }
}
