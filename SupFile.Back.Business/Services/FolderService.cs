namespace SupFile.Back.Business.Services;

public class FolderService : BaseService<Folder, int, IFolderRepository>, IFolderService
{
    private readonly IMediaService _mediaService;

    public FolderService(ILogger<FolderService> logger, IFolderRepository repository, IMediaService mediaService) :
        base(logger, repository)
    {
        _mediaService = mediaService;
    }

    public async Task<Result<Folder>> AddOneAsync(ApplicationUser currentUser, Folder entity)
    {
        entity.OwnerId = currentUser.Id;
        if (entity.ParentId == null)
        {
            return await AddAsync(entity);
        }

        var parentFolderResult = await Repository.GetByIdAsync<Folder>(entity.ParentId.Value);
        if (parentFolderResult.IsFailed)
        {
            return parentFolderResult;
        }

        if (parentFolderResult.Value.IsActive == false)
        {
            return Result.Fail(FolderErrors.CannotAddInSoftDeleted());

            return Result.Fail("Cannot add folder to a soft-deleted parent folder.");
        }
        
        var parentFolder = parentFolderResult.Value;

        if (parentFolder.Id == entity.Id)
        {
            return Result.Fail(FolderErrors.CannotBeOwnParent());
        }

        if (parentFolder.OwnerId != currentUser.Id)
        {
            return Result.Fail(FolderErrors.ParentFolderNotOwnedByUser());
        }

        return await AddAsync(entity);
    }


    public async Task<Result> DeleteOneAsync(ApplicationUser currentUser, int id)
    {
        var folderResult = await Repository.GetByIdAsync<Folder>(id);
        if (folderResult.IsFailed) return folderResult.ToResult();

        if (folderResult.Value.OwnerId != currentUser.Id)
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(id));

        return await DeleteAsync(id);
    }

    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser currentUser)
    {
        return await Repository.DeleteAllSoftDeleted(currentUser);
    }

    public async Task<Result<Tuple<List<Folder>, List<Media>>>> GetFolderContents(ApplicationUser user, int? folderId,
        string? sort)
    {
        var folderResult = await Repository.GetFolderContents<Folder>(user, folderId);
        if (!folderResult.IsSuccess) return folderResult.ToResult();

        var mediaResult = await _mediaService.GetFolderContents<Media>(user, folderId, sort);
        if (!mediaResult.IsSuccess) return mediaResult.ToResult();

        var tuple = Tuple.Create(folderResult.Value, mediaResult.Value);
        return Result.Ok(tuple);
    }

    public async Task<Result<Folder>> UpdateAsync(int id, Folder entity, ApplicationUser currentUser)
    {
        var folderResult = await Repository.GetByIdAsync<Folder>(id);
        if (folderResult.IsFailed) return folderResult;
        var folder = folderResult.Value;
        
        if (folder.OwnerId != currentUser.Id)
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(folder.Id));

        folder.UpdatedDate = DateTime.Now;
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
        if (folderResult.IsFailed) return folderResult.ToResult();

        var folder = folderResult.Value;

        if (folder.OwnerId != user.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(folder.Id));
        }

        if (folder.ParentId is null)
        {
            return Result.Ok(new List<Folder>());
        }

        var pathResult = await Repository.GetPath(user, folder.ParentId);

        return pathResult;
    }

    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser currentUser)
    {
        return await Repository.GetSoftDeleted<TMapped>(currentUser);
    }

    public async Task<Result<Folder>> SoftDeleteAsync(ApplicationUser currentUser, int id)
    {
        var folderResult = await Repository.GetByIdAsync<Folder>(id);
        if (folderResult.IsFailed) return folderResult.ToResult();

        var folder = folderResult.Value;

        if (folder.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(id));
        }

        folder.IsActive = false;

        return await UpdateAsync(id, folder);
    }
    
    public async Task<Result<Folder>> RestoreAsync(ApplicationUser currentUser, int id)
    {
        var folderResult = await Repository.GetByIdAsync<Folder>(id);
        if (folderResult.IsFailed) return folderResult;

        var folder = folderResult.Value;

        if (folder.OwnerId != currentUser.Id)
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(folder.Id));

        folder.IsActive = true;
        
        return await UpdateAsync(id, folder);
    }
}
