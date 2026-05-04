using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Repositories;

public interface IUserRepository: IBaseRepository<ApplicationUser, int>
{
    Task<Result<List<TMapped>>> GetAllUsersAsync<TMapped>(int currentUserId, int pageNumber, int pageSize);
    Task<Result<TMapped?>> GetUserById<TMapped>(int id);
    Task<Result<List<TMapped>>> GetUsersByNameAsync<TMapped>(int currentUserId, string name);
    Task<Result<bool>> DeleteUserAsync(ApplicationUser user);
    Task<Result<List<TMapped>>> GetAccessUsersAsync<TMapped>(int objectId, int currentUserId, ObjectType type);
}