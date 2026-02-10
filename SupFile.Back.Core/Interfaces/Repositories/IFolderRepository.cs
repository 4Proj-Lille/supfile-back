namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IFolderRepository: IBaseRepository<Folder,int>
 {
     Task<Result<List<TMapped>>> GetFrom<TMapped>(ApplicationUser user, int? id);
 }
 
 