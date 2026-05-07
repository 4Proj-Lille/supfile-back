using System.Globalization;
using SupFile.Back.Core.Enums;

namespace SupFile.Back.Business.Services;

public class LinkService : BaseService<Link, int, ILinkRepository>, ILinkService
{
    private readonly IUserService _userService;
    private readonly IMediaService _mediaService;
    private readonly IFolderService _folderService;
    private readonly IShareService _shareService;
    private readonly FrontEndSettings _frontEndSettings;
    private readonly IFluentEmail _fluentEmail;


    public LinkService(ILogger<LinkService> logger, ILinkRepository repository,
        IUserService userService, IMediaService mediaService, IFolderService folderService, IShareService shareService,
        IFluentEmail fluentEmail, IOptions<FrontEndSettings> frontEndSettings) : base(logger,
        repository)
    {
        _userService = userService;
        _mediaService = mediaService;
        _folderService = folderService;
        _fluentEmail = fluentEmail;
        _frontEndSettings = frontEndSettings.Value;
        _shareService = shareService;
    }

    public async Task<Result<string>> GenerateMediaShareLinkAsync(ApplicationUser currentUser, int mediaId)
    {
        var media = await _mediaService.GetByIdAsync<Media>(mediaId);
        if (media.IsFailed) return media.ToResult();

        if (media.Value.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Media, int>(mediaId));
        }

        var token = Guid.NewGuid().ToString();

        var link = new Link
        {
            Token = token,
            Type = InvitationItemType.Media,
            ExpirationDate = DateTime.Now.AddDays(7),
            ShareMediaId = mediaId
        };

        var result = await Repository.AddAsync(link);
        if (result.IsFailed) return result.ToResult();

        var path = Path.Combine(_frontEndSettings.BaseUrl, _frontEndSettings.ShareLink);
        var share = string.Format(CultureInfo.InvariantCulture, path, media.Value.Id, token);

        return share;
    }

    public async Task<Result<string>> GenerateFolderShareLinkAsync(ApplicationUser currentUser, int folderId)
    {
        var folder = await _folderService.GetByIdAsync<Folder>(folderId);
        if (folder.IsFailed) return folder.ToResult();

        if (folder.Value.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(folderId));
        }

        var token = Guid.NewGuid().ToString();

        var link = new Link
        {
            Token = token,
            Type = InvitationItemType.Folder,
            ExpirationDate = DateTime.Now.AddDays(7),
            ShareFolderId = folderId
        };

        var result = await Repository.AddAsync(link);
        if (result.IsFailed) return result.ToResult();

        var path = Path.Combine(_frontEndSettings.BaseUrl, _frontEndSettings.ShareLink);
        var share = string.Format(CultureInfo.InvariantCulture, path,
            folder.Value.Id, token);

        return share;
    }

    public async Task<Result<string>> GenerateEmailShareLinkAsync(ApplicationUser currentUser, int itemId,
        InvitationItemType type,
        int inviteUserId)
    {
        var user = await _userService.GetByIdAsync<ApplicationUser>(inviteUserId);
        if (user.IsFailed)
        {
            return Result.Fail(EntityErrors.NotFoundWithId<ApplicationUser, int>(inviteUserId));
        }

        var generatedLinkResult = type switch
        {
            InvitationItemType.Media => await GenerateMediaShareLinkAsync(currentUser, itemId),
            InvitationItemType.Folder => await GenerateFolderShareLinkAsync(currentUser, itemId),
            _ => Result.Fail(LinkErrors.InvalidType())
        };

        if (generatedLinkResult.IsFailed) return generatedLinkResult;

        await _fluentEmail.To(user.Value.Email).Subject("Invitation to Access Shared Item")
            .Body(
                $" You have been invited to access a shared {type.ToString().ToLower()} by {currentUser.UserName}. Click on the link below to access it: <a href={generatedLinkResult}>Accept Invitation</a>",
                true)
            .SendAsync();

        return Result.Ok(generatedLinkResult.Value);
    }

    public async Task<Result<Link>> GetByTokenAsync(string token)
    {
        var linkResult = await Repository.FindOneAsync<Link>(x => x.Token == token);
        if (linkResult.IsFailed) return linkResult;

        var link = linkResult.Value;

        if (link.ExpirationDate < DateTime.Now)
        {
            return Result.Fail(LinkErrors.LinkExpired());
        }

        return Result.Ok(link);
    }

    public async Task<Result<(byte[], string, string)>> DownloadByTokenAsync(string token)
    {
        var linkResult = await GetByTokenAsync(token);
        if (linkResult.IsFailed) return linkResult.ToResult<(byte[], string, string)>();

        var link = linkResult.Value;

        if (link.ShareMediaId is not null)
        {
            var mediaResult = await _mediaService.GetByIdAsync<Media>(link.ShareMediaId.Value);
            if (mediaResult.IsFailed) return mediaResult.ToResult<(byte[], string, string)>();

            return await _mediaService.DownloadPicture(mediaResult.Value.UniqueId);
        }

        if (link.ShareFolderId is not null)
        {
            var folderResult = await _folderService.DownloadFolderPublicAsync(link.ShareFolderId.Value);
            if (folderResult.IsFailed) return folderResult.ToResult<(byte[], string, string)>();

            var (name, bytes) = folderResult.Value;
            return Result.Ok((bytes, "application/zip", $"{name}.zip"));
        }

        return Result.Fail(LinkErrors.InvalidType());
    }

    public async Task<Result<Share>> AcceptShareLinkAsync(ApplicationUser currentUser, string token)
    {
        var linkResult = await GetByTokenAsync(token);
        if (linkResult.IsFailed) return linkResult.ToResult();

        if (linkResult.Value.ShareFolderId is not null)
        {
            var folderResult = await _folderService.GetByIdAsync<Folder>(linkResult.Value.ShareFolderId.Value);
            if (folderResult.IsFailed) return folderResult.ToResult();

            if (folderResult.Value.OwnerId == currentUser.Id)
            {
                return Result.Fail(LinkErrors.CannotAcceptOwnShareLink());
            }
        }

        if (linkResult.Value.ShareMediaId is not null)
        {
            var mediaResult = await _mediaService.GetByIdAsync<Media>(linkResult.Value.ShareMediaId.Value);
            if (mediaResult.IsFailed) return mediaResult.ToResult();

            if (mediaResult.Value.OwnerId == currentUser.Id)
            {
                return Result.Fail(LinkErrors.CannotAcceptOwnShareLink());
            }
        }

        var share = new Share
        {
            Permission = nameof(SharePermission.Viewer),
            UserId = currentUser.Id,
            Type = linkResult.Value.Type,
            ShareMediaId = linkResult.Value.ShareMediaId,
            ShareFolderId = linkResult.Value.ShareFolderId
        };

        var createResult = await _shareService.AddAsync(share);
        if (createResult.IsFailed) return createResult;

        return Result.Ok(createResult.Value);
    }
}
