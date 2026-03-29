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
        try
        {
            await _storageProvider.WriteAsync(name, extension, bytes);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }

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

    public async Task<Result<List<Media>>> GetFrom(ApplicationUser currentUser, int? id, string? sort)
    {
        var allowedSortFields = new[] { "id", "name", "sendDate", "size", "extension" };

        if (string.IsNullOrEmpty(sort))
        {
            sort = "id";
        }

        if (!allowedSortFields.Contains(sort))
        {
            return Result.Fail(MediaErrors.InvalidSortField(sort));
        }

        var mediaResult = await Repository.GetFrom<Media>(currentUser, id, sort);

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

    public async Task<Result<int>> GetGlobalStorage(ApplicationUser currentUser)
    {
        var storageResult = await Repository.GetGlobalStorage(currentUser);

        return storageResult;
    }

    public async Task<Result<Dictionary<string, int>>> GetStorageByExtension(ApplicationUser currentUser)
    {
        return await Repository.GetStorageByExtension(currentUser);
    }

    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser currentUser)
    {
        return await Repository.GetSoftDeleted<TMapped>(currentUser);
    }

    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser currentUser)
    {
        return await Repository.DeleteAllSoftDeleted(currentUser);
    }

    public async Task<Result<(byte[], string)>> DownloadPicture(string name, string extension)
    {
        var fileResult = await _storageProvider.ReadAsync(name, extension);
        if (fileResult.IsFailed) return fileResult.ToResult();

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType($"{name}.{extension}", out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return Result.Ok((fileResult.Value, contentType));
    }
}
