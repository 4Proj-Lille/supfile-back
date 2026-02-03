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
        
    public async Task<Result<bool>> DeleteOneAsync<TMapped>(ApplicationUser currentUser, int id)
    {
        var shareResult = await Repository.GetByIdAsync<TMapped>(id);
        if (shareResult.IsFailed || shareResult.Value == null)
        {
            return Result.Fail(shareResult.Errors);
        }

        var share = shareResult.Value;

        if (share.Adapt<Share>().UserId == currentUser.Id)
        {
            return await DeleteAsync<TMapped>(id);
        }

        return Result.Fail(new ForbiddenError("You are not authorized to delete this share."));
    }
        

}