namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IShareRepository: IBaseRepository<Share,int>
 {
        Task<Result<List<TMapped>>> GetAllFoldersSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy);
        
        Task<Result<List<TMapped>>> GetAllMediasSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy);
 }
 
 