using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Repositories;

 public interface IShareRepository: IBaseRepository<Share,int>
 {
        Task<Result<List<TMapped>>> GetAllFoldersSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy);

        Task<Result<List<TMapped>>> GetAllMediasSharedAsync<TMapped>(ApplicationUser user, string filter, string orderBy);

        Task<bool> HasMediaPermissionAsync(int mediaId, int userId, SharePermission permission);

        Task<bool> HasFolderPermissionAsync(int folderId, int userId, SharePermission permission);

        Task<Result<Share>> GetByObjectAndUserAsync(int objectId, int targetUserId, InvitationItemType type);
 }
 
 