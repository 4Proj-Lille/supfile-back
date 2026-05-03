using System.Transactions;
using Microsoft.AspNetCore.StaticFiles;
using MimeDetective;
using SupFile.Back.Core.Enums;
using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Business.Services;

public class MediaService : BaseService<Media, int, IMediaRepository>, IMediaService
{
    private readonly IStorageProvider _storageProvider;
    private readonly IFolderRepository _folderRepository;

    public MediaService(
        ILogger<MediaService> logger,
        IMediaRepository repository,
        IStorageProvider storageProvider,
        IFolderRepository folderRepository
    ) : base(logger, repository)
    {
        _storageProvider = storageProvider;
        _folderRepository = folderRepository;
    }

    public async Task<Result<Media>> AddOneAsync(ApplicationUser currentUser, IFormFile file, int? folderId = null)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var name = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();

        var inspector = new ContentInspectorBuilder() {
            Definitions = MimeDetective.Definitions.DefaultDefinitions.All()
        }.Build();

        var results = inspector.Inspect(stream);

        stream.Seek(0, SeekOrigin.Begin);

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();

        Media entity = new()
        {
            Name = name,
            Extension = extension,
            Size = (int)file.Length,
            MimeType = results.ByMimeType().FirstOrDefault()?.MimeType ?? "application/octet-stream",
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
        MediaSearchQuery query)
    {
        var filter = query.ToGridifyFilter();

        var mediaResult = await Repository.GetFolderContents<TMapped>(currentUser, folderId, filter);
        return mediaResult;
    }

    private static readonly HashSet<string> s_includeProps =
    [
        nameof(Media.Name),
        nameof(Media.FolderId),
        nameof(Media.OwnerId),
    ];
    
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

            if (!s_includeProps.Contains(prop.Name))
                continue;
            
            if (!ScalarTypeHelper.IsScalarProperty(prop))
            {
                continue;
            }
            
            var value = prop.GetValue(entity);

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

    public async Task<Result<Dictionary<string, int>>> GetStorageSize(ApplicationUser currentUser,
        StorageSizeGroupBy groupBy)
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

        var fileResult =
            await _storageProvider.ReadAsync(mediaResult.Value.UniqueId.ToString(), mediaResult.Value.Extension);
        if (fileResult.IsFailed) return fileResult.ToResult();

        // var provider = new FileExtensionContentTypeProvider();
        // if (!provider.TryGetContentType($"{mediaResult.Value.UniqueId.ToString()}{mediaResult.Value.Extension}",
        //         out var contentType))
        // {
        //     contentType = "application/octet-stream";
        // }

        if (preview)
        {
            return Result.Ok((fileResult.Value, mediaResult.Value.MimeType,
                $"{mediaResult.Value.UniqueId.ToString()}{mediaResult.Value.Extension}"));
        }

        return Result.Ok((fileResult.Value, mediaResult.Value.MimeType, $"{mediaResult.Value.Name}{mediaResult.Value.Extension}"));
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
        media.UpdatedDate = DateTime.Now;

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

    public async Task<Result<IEnumerable<TMapped>>> SearchAsync<TMapped>(ApplicationUser currentUser,
        MediaSearchQuery query)
    {
        var filter = $"OwnerId={currentUser.Id}";

        var searchFilter = query.ToGridifyFilter();
        if (!string.IsNullOrWhiteSpace(searchFilter))
            filter += $",{searchFilter}";

        var result = await Repository.FindListAsync<TMapped>(filter);
        if (result.IsFailed) return result.ToResult();

        return Result.Ok(result.Value.AsEnumerable());
    }

    public async Task<Result<Dictionary<string, int>>> GetTotalMediaByType(ApplicationUser currentUser)
    {
        var extensionResult = await Repository.GetTotalMediaByExtension(currentUser);
        if (extensionResult.IsFailed) return extensionResult.ToResult();

        var result = extensionResult.Value
            .GroupBy(kvp => MediaTypeHelper.Resolve(kvp.Key))
            .ToDictionary(
                group => group.Key,
                group => group.Sum(kvp => kvp.Value)
            );

        return Result.Ok(result);
    }

    public async Task<Result<int>> SoftDeleteByFolderIdAsync(int folderId)
    {
        var folderIdsResult = await _folderRepository.GetAllDescendantIdsAsync(folderId);
        if (folderIdsResult.IsFailed) return folderIdsResult.ToResult();
        var folderIds = folderIdsResult.Value;
        folderIds.Add(folderId);

        return await Repository.SoftDeleteByFolderIdAsync(folderIds);
    }
    
    public async Task<Result<(byte[], string, string)>> GenerateProfilePictureThumbnail(string userName)
    {
        string first, second;

        var separators = new[] { ' ', '.', '_', '-' };

        if (!string.IsNullOrEmpty(userName) && userName.IndexOfAny(separators) > -1)
        {
            var parts = userName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            first = parts[0];
            second = parts[1];
        }
        else if (!string.IsNullOrEmpty(userName) && userName.Length > 1)
        {
            first = userName[0].ToString();
            second = userName[1].ToString();
        }
        else
        {
            first = userName ?? "A";
            second = "Z";
        }

        var initials = $"{first[0].ToString().ToUpper()}{second[0].ToString().ToUpper()}";

        var svg = $"""
                   <svg xmlns="http://www.w3.org/2000/svg" width="128" height="128">
                       <rect width="128" height="128" fill="#CCCCCC"/>
                       <text x="50%" y="50%" dominant-baseline="central" text-anchor="middle"
                             font-size="52" font-family="Arial" fill="#FFFFFF">{initials}</text>
                   </svg>
                   """;

        var bytes = Encoding.UTF8.GetBytes(svg);
        return Result.Ok((bytes, "image/svg+xml", $"{initials}.svg"));
    }
    
    public async Task<Result<Media>> GetByUniqueIdAsync(Guid uniqueId) 
    {
        return await Repository.GetByUniqueIdAsync(uniqueId);
    }

}
