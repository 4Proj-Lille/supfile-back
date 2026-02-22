using System.Globalization;

namespace SupFile.Back.Business.Services;

public class LinkService : BaseService<Link, int, ILinkRepository>, ILinkService
{
    private readonly IUserService _userService;
    private readonly IMediaService _mediaService;
    private readonly IFolderService _folderService;
    private readonly IShareService _shareService;
    private readonly AppSettings _appSettings;
    private readonly IFluentEmail _fluentEmail;


    public LinkService(ILogger<LinkService> logger, ILinkRepository repository,
        IUserService userService, IMediaService mediaService, IFolderService folderService, IShareService shareService, IOptions<AppSettings> appSettings, IFluentEmail fluentEmail
    ) : base(logger,
        repository)
    {
        _userService = userService;
        _mediaService = mediaService;
        _folderService = folderService;
        _appSettings = appSettings.Value;
        _fluentEmail = fluentEmail;
        _shareService = shareService;
    }

    public async Task<Result<string>> GenerateMediaShareLinkAsync(ApplicationUser currentUser, int mediaId)
    {
        var media = await _mediaService.GetByIdAsync<Media>(mediaId);
        if (media.IsFailed)
        {
            return Result.Fail(media.Errors);
        }
        if (media.Value.OwnerId != currentUser.Id)
        {
            return Result.Fail(new ForbiddenError("You are not the owner of this media"));
        }

        var token = Guid.NewGuid().ToString();
        
        var link = new Link
        {
            
            Token = token,
            Type = "Media",
            ExpirationDate =  DateTime.Now.AddDays(7),
            ShareMediaId =  mediaId
        };
        
        var result = await Repository.AddAsync(link);
        
        var share = string.Format(CultureInfo.InvariantCulture, _appSettings.EmailGenerationFrontendLink, media.Value.Id, token);

        return share;
    }

    public async Task<Result<string>> GenerateFolderShareLinkAsync(ApplicationUser currentUser, int folderId)
    {
        var folder = await _folderService.GetByIdAsync<Folder>(folderId);
        if (folder.IsFailed)
        {
            return Result.Fail(folder.Errors);
        }
        if (folder.Value.OwnerId != currentUser.Id)
        {
            return Result.Fail(new ForbiddenError("You are not the owner of this folder"));
        }

        var token = Guid.NewGuid().ToString();
        
        var link = new Link
        {
            
            Token = token,
            Type = "Folder",
            ExpirationDate =  DateTime.Now.AddDays(7),
            ShareFolderId =  folderId
        };
        
        var result = await Repository.AddAsync(link);
        
        var share = string.Format(CultureInfo.InvariantCulture, _appSettings.EmailGenerationFrontendLink, folder.Value.Id, token);

        return share;
    }

    public async Task<Result<string>> GenerateEmailShareLinkAsync(ApplicationUser currentUser, int itemId, string type, int inviteUserId)
    {
        var user = await _userService.GetByIdAsync<ApplicationUser>(inviteUserId);
        if (user.IsFailed || user.Value == null)
        {
            return Result.Fail(new NotFoundError($"The user with id {inviteUserId} not found"));
        }
        
        var link = type switch
        {
            "Media" => await GenerateMediaShareLinkAsync(currentUser, itemId),
            "Folder" => await GenerateFolderShareLinkAsync(currentUser, itemId),
            _ => Result.Fail(new BadRequestError("Invalid type"))
        };
        
        if (link.IsFailed)
        {
            return Result.Fail(link.Errors);
        }
        
        await _fluentEmail.To(user.Value.Email).Subject("Invitation to Access Shared Item")
            .Body(
                $" You have been invited to access a shared {type.ToLower()} by {currentUser.UserName}. Click on the link below to access it: <a href={link}>Accept Invitation</a>",
                true)
            .SendAsync();
        
        return Result.Ok(link.Value);
    }

    public async Task<Result<Link>> GetByTokenAsync(string token)
    {
        var link = await Repository.GetByTokenAsync(token);
        if (link.IsFailed)
        {
            return Result.Fail(link.Errors);
        }
        
        if (link.Value.ExpirationDate < DateTime.Now)
        {
            return Result.Fail(new Error("This link has expired"));
        }

        return Result.Ok(link.Value);
    }

    public async Task<Result<Share>> AcceptShareLinkAsync(ApplicationUser currentUser, string token)
    {
        var linkResult = await GetByTokenAsync(token);
        if (linkResult.IsFailed)
        {
            return Result.Fail(linkResult.Errors);
        }
        
        if (linkResult.Value.ShareFolderId != null)
        {
            var folderResult = await _folderService.GetByIdAsync<Folder>(linkResult.Value.ShareFolderId.Value);
            if (folderResult.IsFailed)
            {
                return Result.Fail(folderResult.Errors);
            }
            if (folderResult.Value.OwnerId == currentUser.Id)
            {
                return Result.Fail(new ForbiddenError("You cannot accept your own share link"));
            }
        }
        
        if (linkResult.Value.ShareMediaId != null)
        {
            var mediaResult = await _mediaService.GetByIdAsync<Media>(linkResult.Value.ShareMediaId.Value);
            if (mediaResult.IsFailed)
            {
                return Result.Fail(mediaResult.Errors);
            }
            if (mediaResult.Value.OwnerId == currentUser.Id)
            {
                return Result.Fail(new ForbiddenError("You cannot accept your own share link"));
            }
        }

        var share = new Share
        {
            Permission = "Read",
            UserId = currentUser.Id,
            Type = linkResult.Value.Type,
            ShareMediaId = linkResult.Value.ShareMediaId,
            ShareFolderId = linkResult.Value.ShareFolderId
        };

        var result = await _shareService.AddAsync(share);
        if (result.IsFailed)
        {
            return Result.Fail(result.Errors);
        }
        return Result.Ok(result.Value);
    }

}