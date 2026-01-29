namespace SupFile.Back.Business.Services;

public class UserService : BaseService<ApplicationUser, int, IUserRepository>, IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ILogger<UserService> logger, IUserRepository repository,
         UserManager<ApplicationUser> userManager) :
        base(logger, repository)
    {
        _userManager = userManager;
    }
    
    public async Task<Result<ApplicationUser>> AddUser(ApplicationUser user)
    {
        var result = await Repository.AddAsync(user);
        return result;
    }
    
    
    public async Task<Result<List<TMapped>>> GetAllUsersAsync<TMapped>(ApplicationUser currentUser, int pageNumber,
        int pageSize)
    {
        // Add permissions
        var users = await Repository.GetAllUsersAsync<TMapped>(currentUser.Id, pageNumber, pageSize);
        return users;
    }

}