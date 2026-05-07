using SupFile.Back.Core.Enums;

namespace SupFile.Back.Business.Services;

public class ShareService : BaseService<Share, int, IShareRepository>, IShareService
{
    private readonly IUserService _userService;
    private readonly IFolderService _folderService;
    private readonly IMediaRepository _mediaRepository;
    private readonly IFolderRepository _folderRepository;

    public ShareService(ILogger<ShareService> logger, IShareRepository repository,
        IUserService userService, IFolderService folderService,
        IMediaRepository mediaRepository, IFolderRepository folderRepository
    ) : base(logger, repository)
    {
        _userService = userService;
        _folderService = folderService;
        _mediaRepository = mediaRepository;
        _folderRepository = folderRepository;
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
        int ownerId;
        if (dto.Type == InvitationItemType.Media)
        {
            var mediaResult = await _mediaRepository.GetByIdAsync<Media>(dto.ObjectId);
            if (mediaResult.IsFailed) return mediaResult.ToResult<Share>();
            ownerId = mediaResult.Value.OwnerId;
        }
        else
        {
            var folderResult = await _folderRepository.GetByIdAsync<Folder>(dto.ObjectId);
            if (folderResult.IsFailed) return folderResult.ToResult<Share>();
            ownerId = folderResult.Value.OwnerId;
        }

        if (ownerId != currentUser.Id)
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Share, int>(dto.ObjectId));

        var shareResult = await Repository.GetByObjectAndUserAsync(dto.ObjectId, dto.UserId, dto.Type);
        if (shareResult.IsFailed) return shareResult;

        var share = shareResult.Value;
        share.Permission = dto.Permission.ToString();

        return await UpdateAsync(share.Id, share);
    }

    public async Task<Result<Tuple<List<TFolder>, List<TMedia>>>> GetAllAsync<TFolder, TMedia>(ApplicationUser currentUser, SearchQuery query, int? folderId = null, int? size = null)
    {
        var folderFilter = query.ToGridifyFolderFilter();
        var folderOrderBy = query.ToGridifyFolderOrderBy();
        var mediaFilter = query.ToGridifyMediaFilter();
        var mediaOrderBy = query.ToGridifyMediaOrderBy();
        
        if (folderId.HasValue)
        {
            return await _folderService.GetFolderContents<TFolder, TMedia>(currentUser, folderId.Value, query, true, size);
        }

        var folderResult = await Repository.GetAllFoldersSharedAsync<TFolder>(currentUser, folderFilter, folderOrderBy, size);
        var mediaResult = await Repository.GetAllMediasSharedAsync<TMedia>(currentUser, mediaFilter, mediaOrderBy, size);
        
        return Tuple.Create(folderResult.Value, mediaResult.Value);
    }
}
