namespace SupFile.Back.Core.Interfaces.Services;

public interface IFolderService : IBaseService<Folder, int>
{
    
    Task<Result<Folder>> AddOneAsync(ApplicationUser currentUser, Folder entity);

    Task<Result<bool>> DeleteOneAsync<TMapped>(ApplicationUser currentUser, int id);
    
    Task<Result<Tuple<List<Folder>,List<Media>>>> GetFromParent(ApplicationUser user, int? id, string? sort);

    Task<Result<Folder>> UpdateAsync(int id, Folder entity, ApplicationUser currentUser);

}