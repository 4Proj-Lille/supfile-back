using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Services;

public interface ILinkService : IBaseService<Link, int>
{
    Task<Result<string>> GenerateMediaShareLinkAsync(ApplicationUser currentUser, int mediaId, int? targetUserId = null);
    Task<Result<string>> GenerateFolderShareLinkAsync(ApplicationUser currentUser, int folderId, int? targetUserId = null);
    Task<Result<string>> GenerateEmailShareLinkAsync(ApplicationUser currentUser, int itemId, InvitationItemType type, int inviteUserId);
    Task<Result<Share>> AcceptShareLinkAsync(ApplicationUser currentUser, string token);
    Task<Result> DeclineShareLinkAsync(ApplicationUser currentUser, string token);
    Task<Result<List<TMapped>>> GetPendingInvitationsAsync<TMapped>(ApplicationUser currentUser);

    Task<Result<Link>> GetByTokenAsync(string token);

    Task<Result<(byte[], string, string)>> DownloadByTokenAsync(string token);
    Task<Result<List<Link>>> GetExpiredLinksAsync();
}
