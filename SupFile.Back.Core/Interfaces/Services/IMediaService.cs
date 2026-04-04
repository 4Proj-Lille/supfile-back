namespace SupFile.Back.Core.Interfaces.Services;

public interface IMediaService : IBaseService<Media, int>
{
    Task<Result<Media>> AddOneAsync(ApplicationUser currentUser, IFormFile file, int? folderId = null);

    Task<Result> DeleteOneAsync(ApplicationUser currentUser, int id);

    Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser currentUser, int? id, string? sort);
    
    Task<Result<Media>> UpdateAsync(int id, Media entity, ApplicationUser currentUser);
    
    Task<Result<int>> GetTotalStorageSize(ApplicationUser currentUser);

    Task<Result<Dictionary<string, int>>> GetStorageSizeByExtension(ApplicationUser currentUser);
    
    Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser currentUser);
    
    Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser currentUser);

    Task<Result<(byte[], string, string)>> DownloadPicture(int mediaId);
    
    Task<Result<Media>> SoftDeleteAsync(ApplicationUser currentUser, int id);
    
    Task<Result<Media>> RestoreAsync(ApplicationUser currentUser, int id);
}
