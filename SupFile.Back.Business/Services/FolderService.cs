using Microsoft.AspNetCore.Hosting;

namespace SupFile.Back.Business.Services;

public class FolderService : BaseService<Folder, int, IFolderRepository>, IFolderService
{
    private readonly IUserService _userService;
    private readonly IMediaService _mediaService;

    public FolderService(ILogger<FolderService> logger, IFolderRepository repository,
        IUserService userService,  IMediaService mediaService
    ) : base(logger,
        repository)
    {
        _userService = userService;
        _mediaService = mediaService;
    }
    
    public async Task<Result<Folder>> AddOneAsync(ApplicationUser currentUser, Folder entity)
    {
        entity.OwnerId = currentUser.Id;
        var addFolder = await AddAsync(entity);

        return addFolder;
    }

    
    public async Task<Result<bool>> DeleteOneAsync<TMapped>(ApplicationUser currentUser, int id)
    {
        var folderResult = await Repository.GetByIdAsync<TMapped>(id);
        if (folderResult.IsFailed || folderResult.Value == null)
        {
            return Result.Fail(folderResult.Errors);
        }

        var folder = folderResult.Value;

        if (folder.Adapt<Folder>().OwnerId == currentUser.Id)
        {
            return await DeleteAsync<TMapped>(id);
        }

        return Result.Fail(new ForbiddenError("You are not authorized to delete this folder."));
    }
    
    public async Task<Result<Tuple<List<Folder>,List<Media>>>> GetFromRoot(ApplicationUser user)
    {
        var folderResult = await Repository.GetFromRoot<Folder>(user);
        var mediaResult = await _mediaService.GetFromRoot(user);
    
        if (!folderResult.IsSuccess)
            return Result.Fail("Error when getting folders from root");
        
        if (!mediaResult.IsSuccess)
            return Result.Fail("Error when getting medias from root");
    
        var tuple = Tuple.Create(folderResult.Value, mediaResult.Value);
        return Result.Ok(tuple);
    }

}