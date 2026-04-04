namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IFolderRepository: IBaseRepository<Folder,int>
 {
     Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user);

     Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user);
     
     Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? id);
     
     Task<Result<List<Folder>>> GetPath(ApplicationUser user, int? id);
 }
 
 