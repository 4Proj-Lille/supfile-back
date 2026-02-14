namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface ILinkRepository: IBaseRepository<Link,int>
 {
     Task<Result<Link>> GetByTokenAsync(string token);
 }
 
 