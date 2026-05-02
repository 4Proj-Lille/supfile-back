using System.Transactions;
using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Business.Services;

public class FolderService : BaseService<Folder, int, IFolderRepository>, IFolderService
{
    private readonly IMediaService _mediaService;
    private readonly IStorageProvider _storageProvider;


    public FolderService(ILogger<FolderService> logger, IFolderRepository repository, IMediaService mediaService,
        IStorageProvider storageProvider
    ) :
        base(logger, repository)
    {
        _mediaService = mediaService;
        _storageProvider = storageProvider;
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

        if (!parentFolderResult.Value.IsActive)
        {
            return Result.Fail(FolderErrors.CannotAddInSoftDeleted());
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
        MediaSearchQuery query)
    {
        var folderResult = await Repository.GetFolderContents<Folder>(user, folderId);
        if (!folderResult.IsSuccess) return folderResult.ToResult();

        var mediaResult = await _mediaService.GetFolderContents<Media>(user, folderId, query);
        if (!mediaResult.IsSuccess) return mediaResult.ToResult();

        var tuple = Tuple.Create(folderResult.Value, mediaResult.Value);
        return Result.Ok(tuple);
    }

    private static readonly HashSet<string> s_includeProps =
    [
        nameof(Folder.Name),
        nameof(Folder.ParentId),
        nameof(Folder.OwnerId),
    ];
    
    public async Task<Result<Folder>> UpdateAsync(int id, Folder entity, ApplicationUser currentUser)
    {
        var folderResult = await Repository.GetByIdAsync<Folder>(id);
        if (folderResult.IsFailed) return folderResult;
        var folder = folderResult.Value;

        if (folder.OwnerId != currentUser.Id)
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(folder.Id));

        foreach (var prop in typeof(Folder).GetProperties())
        {

            if (!s_includeProps.Contains(prop.Name))
                continue;
            
            if (!ScalarTypeHelper.IsScalarProperty(prop))
            {
                continue;
            }

            var value = prop.GetValue(entity);

            var isDefault = value == null || value.Equals(
                prop.PropertyType.IsValueType 
                    ? Activator.CreateInstance(prop.PropertyType) 
                    : null
            );

            if(prop.Name == nameof(Folder.ParentId) && value is 0)
            {
                isDefault = false;
                value = null;
            }
            
            if (!isDefault)
            {
                prop.SetValue(folder, value);
            }
        }
        
        if (s_includeProps.Contains(nameof(Folder.ParentId)))
        {
            folder.ParentFolder = null;
        }

        if (s_includeProps.Contains(nameof(Folder.OwnerId)))
        {
            folder.OwnerApplicationUserFolder = null!;
        }

        folder.UpdatedDate = DateTime.Now;
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
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var folderResult = await Repository.GetByIdAsync<Folder>(id);
        if (folderResult.IsFailed) return folderResult.ToResult();

        var folder = folderResult.Value;

        if (folder.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(id));
        }

        folder.IsActive = false;
        folder.UpdatedDate = DateTime.Now;

        await _mediaService.SoftDeleteByFolderIdAsync(folder.Id);
        await Repository.SoftDeleteChildrensAsync(folder.Id);
        var updatedFolder = await UpdateAsync(id, folder);

        scope.Complete();

        return updatedFolder;
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

    public async Task<Result<List<int>>> RestoreChainAsync(ApplicationUser currentUser, int folderId)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var folderIdsResult = await Repository.GetAllDescendantIdsAsync(folderId, onlyActive: false);
        if (folderIdsResult.IsFailed) return folderIdsResult;
        var folderIds = folderIdsResult.Value;
        folderIds.Add(folderId);

        var affected = await Repository.RestoreByIdsAsync(folderIds);
        if (affected.IsFailed) return affected.ToResult<List<int>>();

        scope.Complete();
        return Result.Ok(folderIds);
    }

    public async Task<Result<Tuple<string, byte[]>>> DownloadFolderAsync(int folderId, ApplicationUser currentUser)
    {
        var folderResult = await Repository.GetByIdAsync<Folder>(folderId);
        if (folderResult.IsFailed) return folderResult.ToResult();

        if (!folderResult.Value.IsActive)
        {
            return Result.Fail(FolderErrors.CannotDownloadSoftDeleted());
        }

        using var ms = new MemoryStream();
        await using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var buildResult = await AddFolderToArchiveAsync(archive, currentUser, folderId, folderResult.Value.Name);
            if (buildResult.IsFailed)
            {
                return buildResult;
            }
        }

        return Result.Ok(Tuple.Create(folderResult.Value.Name, ms.ToArray()));
    }

    private async Task<Result> AddFolderToArchiveAsync(
        ZipArchive archive,
        ApplicationUser currentUser,
        int folderId,
        string currentPath)
    {
        var mediasResult = await _mediaService.GetFolderContents<Media>(currentUser, folderId, new MediaSearchQuery());
        if (mediasResult.IsFailed) return mediasResult.ToResult();

        foreach (var media in mediasResult.Value)
        {
            var fileResult = await _storageProvider.ReadAsync(media.UniqueId.ToString(), media.Extension);
            if (fileResult.IsFailed) continue;

            var entry = archive.CreateEntry($"{currentPath}/{media.Name}{media.Extension}", CompressionLevel.Fastest);
            await using var entryStream = await entry.OpenAsync();
            await entryStream.WriteAsync(fileResult.Value);
        }

        var subFoldersResult = await Repository.GetFolderContents<Folder>(currentUser, folderId);
        if (!subFoldersResult.IsSuccess) return subFoldersResult.ToResult();

        foreach (var subFolder in subFoldersResult.Value)
        {
            var subPath = $"{currentPath}/{subFolder.Name}";
            var subResult = await AddFolderToArchiveAsync(archive, currentUser, subFolder.Id, subPath);
            if (subResult.IsFailed) return subResult;
        }

        return Result.Ok();
    }
    
    public async Task<Result<long>> GetFolderSizeRecursive(int? folderId, ApplicationUser currentUser)
    {
        if (folderId != null)
        {
            var folderResult = await Repository.GetByIdAsync<Folder>(folderId.Value);
            if (folderResult.IsFailed) return folderResult.ToResult();

            if (folderResult.Value.OwnerId != currentUser.Id)
            {
                return Result.Fail(AuthErrors.UnauthorizedForEntity<Folder, int>(folderId.Value));
            }
        }
        
        return await Repository.GetFolderSizeRecursive(folderId);
    }
}
