namespace SupFile.Back.Core.Interfaces.Services;

public interface IMediaService : IBaseService<Media, int>
{
    
    Task<Result<Media>> AddOneAsync(ApplicationUser currentUser, Media entity);

    Task<Result<bool>> DeleteOneAsync<TMapped>(ApplicationUser currentUser, int id);

    Task<Result<List<Media>>> GetFromRoot(ApplicationUser currentUser);
}
