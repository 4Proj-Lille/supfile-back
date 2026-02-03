using SupFile.Back.Core.Entities.Auth;

namespace SupFile.Back.Data.Repositories;

public class FolderRepository : BaseRepository<Folder, int, SupFileContext>, IFolderRepository
{
    public FolderRepository(ILogger<FolderRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }
    
    public async Task<Result<List<TMapped>>> GetFromRoot<TMapped>(ApplicationUser user)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.ParentId == null
            ).OrderBy(x => x.Name);

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }

}
