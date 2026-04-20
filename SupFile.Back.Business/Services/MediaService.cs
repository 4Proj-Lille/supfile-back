using Microsoft.AspNetCore.StaticFiles;
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
        var name = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        await _storageProvider.WriteAsync(name, extension, bytes);

        Media entity = new()
        {
            Name = name,
            Extension = extension,
            Size = (int)file.Length,
            FolderId = folderId,
            OwnerId = currentUser.Id,
            CreatedDate = DateTime.Now
        };

        var addMedia = await AddAsync(entity);

        if (addMedia.IsFailed || addMedia.Value == null)
        {
            return Result.Fail(addMedia.Errors);
        }

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

        return await DeleteAsync(id);
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

        if (media.OwnerId != currentUser.Id)
        {
            return Result.Fail(AuthErrors.UnauthorizedForEntity<Media, int>(id));
        }

        if (!string.IsNullOrEmpty(entity.Name) && entity.Name != media.Name)
        {
            await _storageProvider.RenameAsync(media.Name, entity.Name, media.Extension);
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

    public async Task<Result<int>> GetTotalStorageSize(ApplicationUser currentUser)
    {
        var storageResult = await Repository.GetTotalStorageSize(currentUser);

        return storageResult;
    }

    public async Task<Result<Dictionary<string, int>>> GetStorageSizeByExtension(ApplicationUser currentUser)
    {
        return await Repository.GetStorageSizeByExtension(currentUser);
    }

    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser currentUser)
    {
        return await Repository.GetSoftDeleted<TMapped>(currentUser);
    }

    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser currentUser)
    {
        return await Repository.DeleteAllSoftDeleted(currentUser);
    }

    public async Task<Result<(byte[], string, string)>> DownloadPicture(int mediaId)
    {
        var mediaResult = await Repository.GetByIdAsync<Media>(mediaId);
        if (mediaResult.IsFailed) return mediaResult.ToResult();

        var fileResult = await _storageProvider.ReadAsync(mediaResult.Value.Name, mediaResult.Value.Extension);
        if (fileResult.IsFailed) return fileResult.ToResult();

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType($"{mediaResult.Value.Name}{mediaResult.Value.Extension}", out var contentType))
        {
            contentType = "application/octet-stream";
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
}
