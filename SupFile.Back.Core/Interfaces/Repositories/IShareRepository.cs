using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IShareRepository: IBaseRepository<Share,int>
 {
        Task<Result<List<TMapped>>> GetAllFoldersSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy, int? limit = null);

        Task<Result<List<TMapped>>> GetAllMediasSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy, int? limit = null);

        Task<bool> HasMediaPermissionAsync(int mediaId, int userId, SharePermission permission);

        Task<bool> HasFolderPermissionAsync(int folderId, int userId, SharePermission permission);
        Task<bool> HasAnyFolderAccessAsync(int folderId, int userId);
        Task<bool> HasAnyMediaAccessAsync(int mediaId, int userId);

        Task<Result<Share>> GetByObjectAndUserAsync(int objectId, int userId, InvitationItemType type);
 }
 
 