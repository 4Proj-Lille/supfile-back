using SupFile.Back.Core.Dto;
using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Services;

public interface IMediaService : IBaseService<Media, int>
{
    Task<Result<Media>> AddOneAsync(ApplicationUser currentUser, IFormFile file, int? folderId = null, string? mediaName = null);

    Task<Result> DeleteOneAsync(ApplicationUser currentUser, int id);
    Task<Result> DeleteByUniqueIdAsync(Guid uniqueId);

    Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser currentUser, int? folderId, SearchQuery query, bool shared = false, int? size = null);
    
    Task<Result<Media>> UpdateAsync(int id, Media entity, ApplicationUser currentUser);
    
    Task<Result<Dictionary<string, int>>> GetTotalStorageSize(ApplicationUser currentUser);

    Task<Result<Dictionary<string, int>>> GetStorageSizeByExtension(ApplicationUser currentUser);
    
    Task<Result<Dictionary<string, int>>> GetStorageSizeByType(ApplicationUser currentUser);

    Task<Result<Dictionary<string, int>>> GetStorageSize(ApplicationUser currentUser, StorageSizeGroupBy groupBy);
    
    
    Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser currentUser);
    
    Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser currentUser);

    Task<Result<(byte[], string, string)>> DownloadPicture(Guid mediaUniqueId, bool preview = false);
    
    Task<Result<Media>> SoftDeleteAsync(ApplicationUser currentUser, int id);
    
    Task<Result<Media>> RestoreAsync(ApplicationUser currentUser, int id);
    
    Task<Result<List<TMapped>>> GetRecentlyModified<TMapped>(ApplicationUser currentUser);

    Task<Result<IEnumerable<TMapped>>> SearchAsync<TMapped>(ApplicationUser currentUser, SearchQuery query);

    Task<Result<Dictionary<string, int>>> GetTotalMediaByType(ApplicationUser currentUser);
    Task<Result<int>> SoftDeleteByFolderIdAsync(int folderId);

    Task<Result<(byte[], string, string)>> GenerateProfilePictureThumbnail(string userName);
    
    Task<Result<Media>> GetByUniqueIdAsync(Guid uniqueId);

    Task<Result> DeleteAllBlobsByUserAsync(int userId);
}
