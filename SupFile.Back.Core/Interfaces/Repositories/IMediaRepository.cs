namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IMediaRepository: IBaseRepository<Media,int>
 {
     Task<Result<List<TMapped>>> GetFrom<TMapped>(ApplicationUser user, int? id);
     
     Task<Result<int>> GetGlobalStorage(ApplicationUser user);

     Task<Result<Dictionary<string, int>>> GetStorageByExtension(ApplicationUser user);
 }
 
 