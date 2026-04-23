using System.Transactions;
using Microsoft.AspNetCore.StaticFiles;
using SupFile.Back.Core.Enums;
using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Business.Services;

public class MediaService : BaseService<Media, int, IMediaRepository>, IMediaService
{
    private readonly IStorageProvider _storageProvider;

    public MediaService(
        ILogger<MediaService> logger,
        IMediaRepository repository,
        IStorageProvider storageProvider
    ) : base(logger, repository)
    {
        _storageProvider = storageProvider;
    }

    public async Task<Result<Media>> AddOneAsync(ApplicationUser currentUser, IFormFile file, int? folderId = null)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var name = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        Media entity = new()
        {
            Name = name,
            Extension = extension,
            Size = (int)file.Length,
            FolderId = folderId,
            OwnerId = currentUser.Id,
        };

        await _storageProvider.WriteAsync(entity.UniqueId.ToString(), extension, bytes);

        var addMedia = await AddAsync(entity);

        if (addMedia.IsFailed || addMedia.Value == null)
        {
            return Result.Fail(addMedia.Errors);
        }
        
        scope.Complete();

        return addMedia;
    }


    public async Task<Result> DeleteOneAsync(ApplicationUser currentUser, int id)
    {
        var mediaResult = await Repository.GetByIdAsync<Media>(id);
        if (mediaResult.IsFailed) return mediaResult.ToResult();

        var media = mediaResult.Value;

        if (media.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Media, int>(id));
        }
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        var deleteBlobMedia = await _storageProvider.DeleteAsync(media.UniqueId.ToString(), media.Extension);
        if (deleteBlobMedia.IsFailed) return deleteBlobMedia;
        
        var deletedMedia = await DeleteAsync(id);
        
        scope.Complete();

        return deletedMedia;
    }

    public async Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser currentUser, int? folderId,
        string? sort)
    {
        // var allowedSortFields = new[] { "id", "name", "sendDate", "size", "extension" };

        if (string.IsNullOrEmpty(sort))
        {
            sort = nameof(Media.Id).ToLower();
        }

        // if (!allowedSortFields.Contains(sort))
        // {
        //     return Result.Fail(MediaErrors.InvalidSortField(sort));
        // }

        var mediaResult = await Repository.GetFolderContents<TMapped>(currentUser, folderId, sort);

        return mediaResult;
    }

    public async Task<Result<Media>> UpdateAsync(int id, Media entity, ApplicationUser currentUser)
    {
        var mediaResult = await Repository.GetByIdAsync<Media>(id);
        if (mediaResult.IsFailed) return mediaResult;

        var media = mediaResult.Value;
        media.UpdatedDate = DateTime.Now;

        if (media.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Media, int>(id));
        }

        foreach (var prop in typeof(Media).GetProperties())
        {
            var value = prop.GetValue(entity);

            if (!ScalarTypeHelper.IsScalarProperty(prop))
            {
                continue;
            }

            if (value != null)
            {
                prop.SetValue(media, value);
            }
        }

        return await UpdateAsync(id, media);
    }

    public async Task<Result<Dictionary<string, int>>> GetTotalStorageSize(ApplicationUser currentUser)
    {
        var storageResult = await Repository.GetTotalStorageSize(currentUser);

        return storageResult;
    }

    public async Task<Result<Dictionary<string, int>>> GetStorageSize (ApplicationUser currentUser, StorageSizeGroupBy groupBy)
    {
        return groupBy switch
        {
            StorageSizeGroupBy.Extension => await GetStorageSizeByExtension(currentUser),
            StorageSizeGroupBy.Type => await GetStorageSizeByType(currentUser),
            StorageSizeGroupBy.Global => await GetTotalStorageSize(currentUser),
            _ => Result.Fail(MediaErrors.InvalidStorageSizeGroupBy())
        };
    }
        
    public async Task<Result<Dictionary<string, int>>> GetStorageSizeByExtension(ApplicationUser currentUser)
    {
        return await Repository.GetStorageSizeByExtension(currentUser);
    }

    public async Task<Result<Dictionary<string, int>>> GetStorageSizeByType(ApplicationUser currentUser)
    {
        var extensionResult = await Repository.GetStorageSizeByExtension(currentUser);
        if (extensionResult.IsFailed) return extensionResult;

        var result = extensionResult.Value
            .GroupBy(kvp => MediaTypeHelper.Resolve(kvp.Key))
            .ToDictionary(
                group => group.Key,
                group => group.Sum(kvp => kvp.Value)
            );
        return Result.Ok(result);
    }
    
    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser currentUser)
    {
        return await Repository.GetSoftDeleted<TMapped>(currentUser);
    }

    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser currentUser)
    {
        return await Repository.DeleteAllSoftDeleted(currentUser);
    }

    public async Task<Result<(byte[], string, string)>> DownloadPicture(Guid mediaUniqueId, bool preview = false)
    {
        var mediaResult = await Repository.GetByUniqueIdAsync(mediaUniqueId);
        if (mediaResult.IsFailed) return mediaResult.ToResult();

        var fileResult = await _storageProvider.ReadAsync(mediaResult.Value.UniqueId.ToString(), mediaResult.Value.Extension);
        if (fileResult.IsFailed) return fileResult.ToResult();

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType($"{mediaResult.Value.UniqueId.ToString()}{mediaResult.Value.Extension}", out var contentType))
        {
            contentType = "application/octet-stream";
        }

        if (preview)
        {
            return Result.Ok((fileResult.Value, contentType, $"{mediaResult.Value.UniqueId.ToString()}{mediaResult.Value.Extension}"));
        }
        return Result.Ok((fileResult.Value, contentType, $"{mediaResult.Value.Name}{mediaResult.Value.Extension}"));
    }

    public async Task<Result<Media>> SoftDeleteAsync(ApplicationUser currentUser, int id)
    {
        var mediaResult = await Repository.GetByIdAsync<Media>(id);
        if (mediaResult.IsFailed) return mediaResult.ToResult();

        var media = mediaResult.Value;

        if (media.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Media, int>(id));
        }

        media.IsActive = false;

        return await UpdateAsync(id, media);
    }

    public async Task<Result<Media>> RestoreAsync(ApplicationUser currentUser, int id)
    {
        var mediaResult = await Repository.GetByIdAsync<Media>(id);
        if (mediaResult.IsFailed) return mediaResult.ToResult();

        var media = mediaResult.Value;

        if (media.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Media, int>(id));
        }

        media.IsActive = true;

        var updateMedia = await UpdateAsync(id, media);

        if (updateMedia.IsFailed || updateMedia.Value == null)
        {
            return Result.Fail(updateMedia.Errors);
        }

        return updateMedia;
    }
    
    public async Task<Result<List<TMapped>>> GetRecentlyModified<TMapped>(ApplicationUser currentUser)
    {
        return await Repository.GetRecentlyModified<TMapped>(currentUser);
    }
}
