namespace SupFile.Back.Core.Interfaces.Services;

public interface IMediaService : IBaseService<Media, int>
{
    Task<Result<Media>> AddOneAsync(ApplicationUser currentUser, IFormFile file, int? folderId = null);

    Task<Result<bool>> DeleteOneAsync<TMapped>(ApplicationUser currentUser, int id);

    Task<Result<List<Media>>> GetFrom(ApplicationUser currentUser, int? id, string? sort);
    
    Task<Result<Media>> UpdateAsync(int id, Media entity, ApplicationUser currentUser);
    
    Task<Result<int>> GetGlobalStorage(ApplicationUser currentUser);

    Task<Result<Dictionary<string, int>>> GetStorageByExtension(ApplicationUser currentUser);
    
    Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser currentUser);
    
    Task<Result<bool>> DeleteAllSoftDeleted(ApplicationUser currentUser);

    Task<Result<(byte[], string)>> DownloadPicture(string name, string extension);
}
