namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IFolderRepository: IBaseRepository<Folder,int>
 {
     Task<Result<List<TMapped>>> GetFromRoot<TMapped>(ApplicationUser user);
 }
 
 