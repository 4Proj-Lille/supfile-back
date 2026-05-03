using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Services;

public interface IShareService : IBaseService<Share, int>
{
    Task<Result<Share>> AddOneAsync(ApplicationUser currentUser, Share entity);

    Task<Result> DeleteOneAsync(ApplicationUser currentUser, int id);
    
    Task<Result<List<TMapped>>> GetAccessUsersAsync<TMapped>(int objectId, ApplicationUser currentUser, ObjectType type);
    
    Task<Result<TMapped>> GetAllAsync<TMapped>(ApplicationUser currentUser);
}
