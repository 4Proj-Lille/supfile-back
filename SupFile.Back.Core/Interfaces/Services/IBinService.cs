using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Interfaces.Services;

public interface IBinService
{
    
    Task<Result> RestoreAsync(int id, ApplicationUser currentUser, ObjectType type);
    
    Task<Result> DeleteOneAsync(int id, ApplicationUser currentUser, ObjectType type);
    
    Task<Result> EmptyBinAsync(ApplicationUser currentUser);
    
    Task<Result<Tuple<List<Folder>,List<Media>>>> GetBinItemsAsync(ApplicationUser currentUser, ObjectType? type);
}
