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

    public async Task<Result<Tuple<List<Folder>, List<Media>>>> GetFromParent(ApplicationUser user, int? id,
        string? sort)
    {
        var folderResult = await Repository.GetFrom<Folder>(user, id);
        if (!folderResult.IsSuccess) return folderResult.ToResult();

        var mediaResult = await _mediaService.GetFrom(user, id, sort);
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
}
