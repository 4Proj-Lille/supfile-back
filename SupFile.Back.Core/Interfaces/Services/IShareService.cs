namespace SupFile.Back.Core.Interfaces.Services;

public interface IShareService : IBaseService<Share, int>
{
    Task<Result<Share>> AddOneAsync(ApplicationUser currentUser, Share entity);

    Task<Result<bool>> DeleteOneAsync<TMapped>(ApplicationUser currentUser, int id);
    
}
