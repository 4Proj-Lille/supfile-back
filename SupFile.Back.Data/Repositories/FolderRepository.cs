using SupFile.Back.Core.Entities.Auth;
using SupFile.Back.Core.Errors;

namespace SupFile.Back.Data.Repositories;

public class FolderRepository : BaseRepository<Folder, int, SupFileContext>, IFolderRepository
{
    public FolderRepository(ILogger<FolderRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }
    
    public async Task<Result<int>> DeleteAllSoftDeleted(ApplicationUser user)
    {
        return await DeleteAllAsync(x => x.OwnerId == user.Id && !x.IsActive);
    }

    public async Task<Result<List<TMapped>>> GetFolderContents<TMapped>(ApplicationUser user, int? id)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.ParentId == id
        ).OrderBy(x => x.Name);

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }

    public async Task<Result<List<Folder>>> GetPath(ApplicationUser user, int? id)
    {
        var path = new List<Folder>();
        var currentId = id;

        while (currentId != null)
        {
            var folderResult =
                await Query().Where(x => x.Id == currentId && x.OwnerId == user.Id).FirstOrDefaultAsync();
            if (folderResult == null)
            {
                return Result.Fail(EntityErrors.NotFound<Folder>());
            }

            path.Add(folderResult);
            currentId = folderResult.ParentId;
        }

        path.Reverse();
        return Result.Ok(path);
    }
    
    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && !x.IsActive
        );

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }
}
