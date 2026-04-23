namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IMediaRepository: IBaseRepository<Media,int>
 {
     Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? folderId, string filter);
     
     Task<Result<Dictionary<string, int>>> GetTotalStorageSize(ApplicationUser user);

     Task<Result<Dictionary<string, int>>> GetStorageSizeByExtension(ApplicationUser user);
     
     Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user);
     
     Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user);
     
     Task<Result<Media>> GetByUniqueIdAsync(Guid uniqueId);
     
     Task<Result<List<TMapped>>> GetRecentlyModified<TMapped>(ApplicationUser user);

     Task<Result<int>> GetTotalMediaByType(ApplicationUser currentUser, string filter);
 }
 
 