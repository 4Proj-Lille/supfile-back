namespace SupFile.Back.Core.Interfaces.Services;

public interface IFolderService : IBaseService<Folder, int>
{
    
    Task<Result<Folder>> AddOneAsync(ApplicationUser currentUser, Folder entity);

    Task<Result> DeleteOneAsync(ApplicationUser currentUser, int id);

    Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser currentUser);
    
    Task<Result<Tuple<List<Folder>,List<Media>>>> GetFolderContents(ApplicationUser user, int? folderId, string? sort);

    Task<Result<Folder>> UpdateAsync(int id, Folder entity, ApplicationUser currentUser);
    
    Task<Result<List<Folder>>> GetPath(ApplicationUser user, int id);

    Task<Result<Folder>> SoftDeleteAsync(ApplicationUser currentUser, int id);
    
    Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser currentUser);
    
    Task<Result<Folder>> RestoreAsync(ApplicationUser currentUser, int id);

    Task<Result<Tuple<string,byte[]>>> DownloadFolderAsync(int folderId, ApplicationUser currentUser);
}