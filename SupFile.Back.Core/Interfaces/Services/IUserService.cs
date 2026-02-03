namespace SupFile.Back.Core.Interfaces.Services;

public interface IUserService : IBaseService<ApplicationUser, int>
{
    Task<Result<ApplicationUser>> AddUser(ApplicationUser user);
    // Task<Result> DeleteUserAsync(AuthIdentityUser currentUser, int userId);
    Task<Result<List<TMapped>>> GetAllUsersAsync<TMapped>(ApplicationUser currentUser, int pageNumber, int pageSize);
}
