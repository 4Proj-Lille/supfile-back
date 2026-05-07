namespace SupFile.Back.Core.Interfaces.Repositories;

public interface IFolderRepository : IBaseRepository<Folder, int>
{
    Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user);

    Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user);

    Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? id, string filter, string orderBy, bool shared = false, int? limit = null);

    Task<Result<List<TMapped>>> GetPath<TMapped>(ApplicationUser user, int? id);
    Task<Result<int>> SoftDeleteChildrensAsync(int folderId);
    Task<Result<List<int>>> GetAllDescendantIdsAsync(int folderId, bool onlyActive = true);
    Task<Result<int>> RestoreByIdsAsync(List<int> folderIds);

    Task<Result<long>> GetFolderSizeRecursive(int? folderId);

}
