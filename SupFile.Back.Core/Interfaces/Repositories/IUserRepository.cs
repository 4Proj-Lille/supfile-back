namespace SupFile.Back.Core.Interfaces.Repositories;

public interface IUserRepository: IBaseRepository<ApplicationUser, int>
{
    Task<Result<List<TMapped>>> GetAllUsersAsync<TMapped>(int currentUserId, int pageNumber, int pageSize);
    Task<Result<TMapped?>> GetUserById<TMapped>(int id);
    Task<Result<bool>> DeleteUserAsync(ApplicationUser user);
}