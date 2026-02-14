namespace SupFile.Back.Core.Interfaces.Services;

public interface ILinkService : IBaseService<Link, int>
{
    Task<Result<string>> GenerateMediaShareLinkAsync(ApplicationUser currentUser, int mediaId); 
    Task<Result<string>> GenerateFolderShareLinkAsync(ApplicationUser currentUser, int folderId);
    Task<Result<string>> GenerateEmailShareLinkAsync(ApplicationUser currentUser, int itemId, string type, int inviteUserId);
    Task<Result<Share>> AcceptShareLinkAsync(ApplicationUser currentUser, string token);
    
    Task<Result<Link>> GetByTokenAsync(string token);
}
