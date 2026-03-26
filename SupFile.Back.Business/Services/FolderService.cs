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
        if (entity.ParentId != null)
        {
            var parentFolderResult = await Repository.GetByIdAsync<Folder>(entity.ParentId.Value);
            if (parentFolderResult.IsFailed || parentFolderResult.Value == null)
            {
                return Result.Fail(parentFolderResult.Errors);
            }

            var parentFolder = parentFolderResult.Value;
            
            if (parentFolder.Id == entity.Id)
            {
                return Result.Fail(new BadRequestError("A folder cannot be its own parent."));
            }
            
            if (parentFolder.OwnerId != currentUser.Id)
            {
                return Result.Fail(new ForbiddenError("You are not authorized to add a folder to this parent."));
            }
        }
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
    
    public async Task<Result<Tuple<List<Folder>,List<Media>>>> GetFromParent(ApplicationUser user, int? id, string? sort)
    {
        var folderResult = await Repository.GetFrom<Folder>(user, id);
        var mediaResult = await _mediaService.GetFrom(user, id, sort);
    
        if (!folderResult.IsSuccess)
            return Result.Fail(folderResult.Errors);
        
        if (!mediaResult.IsSuccess)
            return Result.Fail(mediaResult.Errors);
    
        var tuple = Tuple.Create(folderResult.Value, mediaResult.Value);
        return Result.Ok(tuple);
    }

    public async Task<Result<Folder>> UpdateAsync(int id, Folder entity, ApplicationUser currentUser)
    {
        var folderResult = await Repository.GetByIdAsync<Folder>(id);
        if (folderResult.IsFailed || folderResult.Value == null)
        {
            return folderResult;
        }

        var folder = folderResult.Value;

        if ( folder.OwnerId != currentUser.Id)
        {
            return Result.Fail(new ForbiddenError("You are not authorized to update this folder."));
        }

        foreach (var prop in typeof(Folder).GetProperties())
        {
            var value = prop.GetValue(entity);

            if (!ScalarTypeHelper.IsScalarProperty(prop))
            {
                continue;
            }

            if (value != null)
            {
                prop.SetValue(folder, value);
            }
        }

        return await UpdateAsync(id, folder);
    }
    
    public async Task<Result<List<Folder>>> GetPath(ApplicationUser user, int id)
    {
        var folderResult = await Repository.GetByIdAsync<Folder>(id);
        if (folderResult.IsFailed || folderResult.Value == null)
        {
            return Result.Fail(folderResult.Errors);
        }

        var folder = folderResult.Value;

        if (folder.OwnerId != user.Id)
        {
            return Result.Fail(new ForbiddenError("You are not authorized to access this folder."));
        }
        if (folder.ParentId == null)
        {
            return Result.Ok(new List<Folder>());
        }
        var pathResult = await Repository.GetPath(user, folder.ParentId);

        return pathResult;
    }
}