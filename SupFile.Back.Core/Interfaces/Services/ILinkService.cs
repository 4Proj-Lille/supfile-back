namespace SupFile.Back.Core.Interfaces.Services;

public interface ILinkService : IBaseService<Link, int>
{
    
    Task<Result<Link>> GenerateLinkAsync(Link entity);
    

}
