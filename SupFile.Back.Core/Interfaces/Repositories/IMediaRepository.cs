namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IMediaRepository: IBaseRepository<Media,int>
 {
     Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? folderId, string sort);
     
     Task<Result<int>> GetTotalStorageSize(ApplicationUser user);

     Task<Result<Dictionary<string, int>>> GetStorageSizeByExtension(ApplicationUser user);
     
     Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user);
     
     Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user);
 }
 
 