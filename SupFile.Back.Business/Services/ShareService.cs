using SupFile.Back.Core.Enums;

namespace SupFile.Back.Business.Services;

public class ShareService : BaseService<Share, int, IShareRepository>, IShareService
{
    private readonly IUserService _userService;

    public ShareService(ILogger<ShareService> logger, IShareRepository repository,
        IUserService userService
    ) : base(logger,
        repository)
    {
        _userService = userService;
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
}
