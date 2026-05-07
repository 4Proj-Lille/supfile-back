using System.Transactions;
using MimeDetective;
using SupFile.Back.Core.Enums;
using SupFile.Back.Storage.Interfaces;

namespace SupFile.Back.Business.Services;

public class MediaService : BaseService<Media, int, IMediaRepository>, IMediaService
{
    private readonly IStorageProvider _storageProvider;
    private readonly IFolderRepository _folderRepository;
    private readonly AppSettings _appSettings;

    public MediaService(
        ILogger<MediaService> logger,
        IMediaRepository repository,
        IStorageProvider storageProvider,
        IFolderRepository folderRepository,
        IOptions<AppSettings> appSettings
    ) : base(logger, repository)
    {
        _storageProvider = storageProvider;
        _folderRepository = folderRepository;
        _appSettings = appSettings.Value;
    }

    public async Task<Result<Media>> AddOneAsync(ApplicationUser currentUser, IFormFile file, int? folderId = null, string? mediaName = null)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        var storageSizeResult = await Repository.GetTotalStorageSize(currentUser);
        if (storageSizeResult.IsFailed) return storageSizeResult.ToResult();
        var totalStorageSize = storageSizeResult.Value.Values.FirstOrDefault();
        if (totalStorageSize + file.Length > _appSettings.AllocatedSpace)
        {
            return Result.Fail(MediaErrors.StorageLimitExceeded());
        }
        
        var name = mediaName ?? Path.GetFileNameWithoutExtension(file.FileName);
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

    public async Task<Result> DeleteByUniqueIdAsync(Guid uniqueId)
    {
        var mediaResult = await Repository.GetByUniqueIdAsync(uniqueId);
        if (mediaResult.IsFailed) return mediaResult.ToResult();

        var media = mediaResult.Value;

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var deleteBlobResult = await _storageProvider.DeleteAsync(media.UniqueId.ToString(), media.Extension);
        if (deleteBlobResult.IsFailed) return deleteBlobResult;

        var deleteDbResult = await DeleteAsync(media.Id);

        scope.Complete();

        return deleteDbResult;
    }

    public async Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser currentUser, int? folderId,
        SearchQuery query, bool shared = false)
    {
        var filter = query.ToGridifyMediaFilter();
        var orderBy = query.ToGridifyMediaOrderBy();

        var mediaResult = await Repository.GetFolderContents<TMapped>(currentUser, folderId, filter, orderBy, shared);
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

            var isDefault = value == null || value.Equals(
                prop.PropertyType.IsValueType 
                    ? Activator.CreateInstance(prop.PropertyType) 
                    : null
            );
            
            if(prop.Name == nameof(Media.FolderId) && value is 0)
            {
                isDefault = false;
                value = null;
            }
            
            if (!isDefault)
            {
                prop.SetValue(media, value);
            }
            
        }
        
        if (s_includeProps.Contains(nameof(Media.FolderId)))
        {
            media.Folder = null;
        }

        if (s_includeProps.Contains(nameof(Media.OwnerId)))
        {
            media.OwnerApplicationUser = null!;
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
        SearchQuery query)
    {
        var filter = $"OwnerId={currentUser.Id}";

        if (currentUser.ProfilePictureId != null)
            filter += $",UniqueId!={currentUser.ProfilePictureId}";

        var orderBy = query.ToGridifyMediaOrderBy();
        
        var searchFilter = query.ToGridifyMediaFilter();
        if (!string.IsNullOrWhiteSpace(searchFilter))
            filter += $",{searchFilter}";

        var result = await Repository.FindListAsync<TMapped>(filter, orderBy: orderBy);
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

        var initials = $"{char.ToUpper(first[0])}{char.ToUpper(second[0])}";
        var color = GetColorFromString(userName ?? initials);

        var svg = $"""
                   <svg xmlns="http://www.w3.org/2000/svg" width="128" height="128">
                       <rect width="128" height="128" fill="{color}"/>
                       <text x="50%" y="50%" dominant-baseline="central" text-anchor="middle"
                             font-size="52" font-family="Arial" fill="#FFFFFF">{initials}</text>
                   </svg>
                   """;

        var bytes = Encoding.UTF8.GetBytes(svg);
        return Result.Ok((bytes, "image/svg+xml", $"{initials}.svg"));
    }

    private static string GetColorFromString(string input)
    {
        var hash = input.Aggregate(0, (acc, c) => acc * 31 + c);
        var hue = Math.Abs(hash) % 360;
        return HslToHex(hue, saturation: 0.55, lightness: 0.45);
    }

    private static string HslToHex(int hue, double saturation, double lightness)
    {
        var c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        var x = c * (1 - Math.Abs(hue / 60.0 % 2 - 1));
        var m = lightness - c / 2;

        double r, g, b;
        if      (hue < 60)  { r = c; g = x; b = 0; }
        else if (hue < 120) { r = x; g = c; b = 0; }
        else if (hue < 180) { r = 0; g = c; b = x; }
        else if (hue < 240) { r = 0; g = x; b = c; }
        else if (hue < 300) { r = x; g = 0; b = c; }
        else                { r = c; g = 0; b = x; }

        return $"#{(int)((r + m) * 255):X2}{(int)((g + m) * 255):X2}{(int)((b + m) * 255):X2}";
    }
    
    public async Task<Result<Media>> GetByUniqueIdAsync(Guid uniqueId)
    {
        return await Repository.GetByUniqueIdAsync(uniqueId);
    }

    public async Task<Result> DeleteAllBlobsByUserAsync(int userId)
    {
        var mediasResult = await Repository.GetAllByUserIdAsync(userId);
        if (mediasResult.IsFailed) return mediasResult.ToResult();

        var errors = new List<IError>();
        foreach (var media in mediasResult.Value)
        {
            var deleteResult = await _storageProvider.DeleteAsync(media.UniqueId.ToString(), media.Extension);
            if (deleteResult.IsFailed)
                errors.AddRange(deleteResult.Errors);
        }

        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok();
    }

}
