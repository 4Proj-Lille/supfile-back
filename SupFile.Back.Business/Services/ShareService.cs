using SupFile.Back.Core.Enums;

namespace SupFile.Back.Business.Services;

public class ShareService : BaseService<Share, int, IShareRepository>, IShareService
{
    private readonly IUserService _userService;
    private readonly IFolderService _folderService;

    public ShareService(ILogger<ShareService> logger, IShareRepository repository,
        IUserService userService, IFolderService folderService
    ) : base(logger,
        repository)
    {
        _userService = userService;
        _folderService = folderService;
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

    public async Task<Result<Tuple<List<TFolder>, List<TMedia>>>> GetAllAsync<TFolder, TMedia>(ApplicationUser currentUser, SearchQuery query, int? folderId = null)
    {
        var folderFilter = query.ToGridifyFolderFilter();
        var folderOrderBy = query.ToGridifyFolderOrderBy();
        var mediaFilter = query.ToGridifyMediaFilter();
        var mediaOrderBy = query.ToGridifyMediaOrderBy();
        
        if (folderId.HasValue)
        {
            return await _folderService.GetFolderContents<TFolder, TMedia>(currentUser, folderId.Value, query, true);
        }

        var folderResult = await Repository.GetAllFoldersSharedAsync<TFolder>(currentUser, folderFilter, folderOrderBy);
        var mediaResult = await Repository.GetAllMediasSharedAsync<TMedia>(currentUser, mediaFilter, mediaOrderBy);
        
        return Tuple.Create(folderResult.Value, mediaResult.Value);
    }
}
