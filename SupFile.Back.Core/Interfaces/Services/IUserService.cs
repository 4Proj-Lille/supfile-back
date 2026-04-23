namespace SupFile.Back.Core.Interfaces.Services;

public interface IUserService : IBaseService<ApplicationUser, int>
{
    Task<Result<ApplicationUser>> AddUser(ApplicationUser user);

    Task<Result<List<TMapped>>> GetAllUsersAsync<TMapped>(ApplicationUser currentUser, int pageNumber, int pageSize);
    
    Task<Result<ApplicationUser>> UpdateAsync(int userId, ApplicationUser entity, ApplicationUser currentUser, IFormFile? profilePicture);
    
    Task<Result<ApplicationUser>> UpdatePasswordAsync(int userId, string currentPassword, string newPassword, string confirmNewPassword, ApplicationUser currentUser);
    
    Task<Result> DeleteUserAsync(ApplicationUser currentUser, int userId);

}
