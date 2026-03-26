namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IFolderRepository: IBaseRepository<Folder,int>
 {
     Task<Result<List<TMapped>>> GetFrom<TMapped>(ApplicationUser user, int? id);
     
     Task<Result<List<Folder>>> GetPath(ApplicationUser user, int? id);
 }
 
 