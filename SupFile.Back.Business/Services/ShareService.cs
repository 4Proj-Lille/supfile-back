using SupFile.Back.Core.Enums;

namespace SupFile.Back.Business.Services;

public class ShareService : BaseService<Share, int, IShareRepository>, IShareService
{
    private readonly IUserService _userService;
    private readonly IFolderService _folderService;
    private readonly IMediaRepository _mediaRepository;

    public ShareService(ILogger<ShareService> logger, IShareRepository repository,
        IUserService userService, IFolderService folderService,
        IMediaRepository mediaRepository
    ) : base(logger, repository)
    {
        _userService = userService;
        _folderService = folderService;
        _mediaRepository = mediaRepository;
    }

    public async Task<Result<Share>> AddOneAsync(ApplicationUser currentUser, Share entity)
    {
        var addFolder = await AddAsync(entity);

        return addFolder;
    }

    public async Task<Result> DeleteOneAsync(ApplicationUser currentUser, int id)
    {
        var shareResult = await Repository.GetByIdAsync<Share>(id);
        if (shareResult.IsFailed) return shareResult.ToResult();

        var share = shareResult.Value;

        if (share.Adapt<Share>().UserId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Share, int>(id));
        }

        return await DeleteAsync(id);
    }
    
    public async Task<Result<List<TMapped>>> GetAccessUsersAsync<TMapped>(int objectId, ApplicationUser currentUser, ObjectType type)
    {
        var accessUsersResult = await _userService.GetAccessUsersAsync<TMapped>(objectId, currentUser, type);
        return accessUsersResult;
    }

    public async Task<Result<Share>> UpdatePermissionAsync(ApplicationUser currentUser, UpdateSharePermissionDto dto)
    {
        var ownerResult = await GetOwnerIdAsync(dto.ObjectId, dto.Type);
        if (ownerResult.IsFailed) return ownerResult.ToResult<Share>();

        if (ownerResult.Value != currentUser.Id)
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Share, int>(dto.ObjectId));

        var shareResult = await Repository.GetByObjectAndUserAsync(dto.ObjectId, dto.UserId, dto.Type);
        if (shareResult.IsFailed) return shareResult;

        var share = shareResult.Value;
        share.Permission = dto.Permission.ToString();

        return await UpdateAsync(share.Id, share);
    }

    public async Task<Result> RevokeAccessAsync(ApplicationUser currentUser, int objectId, int userId, InvitationItemType type)
    {
        var ownerResult = await GetOwnerIdAsync(objectId, type);
        if (ownerResult.IsFailed) return ownerResult.ToResult();

        var isOwner = ownerResult.Value == currentUser.Id;
        var isSelfRevoke = userId == currentUser.Id;

        if (!isOwner && !isSelfRevoke)
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Share, int>(objectId));

        var shareResult = await Repository.GetByObjectAndUserAsync(objectId, userId, type);
        if (shareResult.IsFailed) return shareResult.ToResult();

        return await DeleteAsync(shareResult.Value.Id);
    }

    private async Task<Result<int>> GetOwnerIdAsync(int objectId, InvitationItemType type)
    {
        if (type == InvitationItemType.Media)
        {
            var mediaResult = await _mediaRepository.GetByIdAsync<Media>(objectId);
            if (mediaResult.IsFailed) return mediaResult.ToResult<int>();
            return Result.Ok(mediaResult.Value.OwnerId);
        }

        var folderResult = await _folderService.GetByIdAsync<Folder>(objectId);
        if (folderResult.IsFailed) return folderResult.ToResult<int>();
        return Result.Ok(folderResult.Value.OwnerId);
    }

    public async Task<Result<Tuple<List<TFolder>, List<TMedia>>>> GetAllAsync<TFolder, TMedia>(ApplicationUser currentUser, SearchQuery query, int? folderId = null, int? limit = null)
    {
        var folderFilter = query.ToGridifyFolderFilter();
        var folderOrderBy = query.ToGridifyFolderOrderBy();
        var mediaFilter = query.ToGridifyMediaFilter();
        var mediaOrderBy = query.ToGridifyMediaOrderBy();
        
        if (folderId.HasValue)
        {
            return await _folderService.GetFolderContents<TFolder, TMedia>(currentUser, folderId.Value, query, true, limit);
        }

        var folderResult = await Repository.GetAllFoldersSharedAsync<TFolder>(currentUser, folderFilter, folderOrderBy, limit);
        var mediaResult = await Repository.GetAllMediasSharedAsync<TMedia>(currentUser, mediaFilter, mediaOrderBy, limit);
        
        return Tuple.Create(folderResult.Value, mediaResult.Value);
    }
    
    public async Task<bool> HasAnyFolderAccessAsync(int folderId, int userId)
    {
        return await Repository.HasAnyFolderAccessAsync(folderId, userId);
    }
    
    public async Task<bool> HasAnyMediaAccessAsync(int mediaId, int userId)
    {
        return await Repository.HasAnyMediaAccessAsync(mediaId, userId);
    }
}
