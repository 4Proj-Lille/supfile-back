using SupFile.Back.Core.Entities.Auth;
using System.Linq.Dynamic.Core;

namespace SupFile.Back.Data.Repositories;

public class MediaRepository : BaseRepository<Media, int, SupFileContext>, IMediaRepository
{
    public MediaRepository(ILogger<MediaRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<List<TMapped>>> GetFrom<TMapped>(ApplicationUser user, int? id, string sort)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.FolderId == id
        ).OrderBy(sort);

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }
    
    public async Task<Result<int>> GetGlobalStorage(ApplicationUser user){
        
        return Result.Ok(
            await Query()
                .Where(x => x.OwnerId == user.Id)
                .SumAsync(x => x.Size)
        );
    }

    public async Task<Result<Dictionary<string, int>>> GetStorageByExtension(ApplicationUser user)
    {
        var storageByExtension = await Query()
            .Where(x => x.OwnerId == user.Id)
            .GroupBy(x => x.Extension)
            .Select(g => new 
            { 
                Extension = g.Key, 
                TotalSize = g.Sum(x => x.Size) 
            })
            .ToDictionaryAsync(x => x.Extension, x => x.TotalSize);

        return Result.Ok(storageByExtension);
    }
    
    public async Task<Result<List<TMapped>>> GetSoftDeleted<TMapped>(ApplicationUser user)
    {
        var q = Query().Where(x =>
            x.OwnerId == user.Id && x.IsActive
        );

        return Result.Ok(await q.FindListAsync<TMapped>(""));
    }
    
    public async Task<Result<bool>> DeleteAllSoftDeleted(ApplicationUser user)
    {
        var softDeletedMedias = await Query()
            .Where(x => x.OwnerId == user.Id && !x.IsActive)
            .ToListAsync();

        if (softDeletedMedias.Count == 0)
        {
            return Result.Ok(true);
        }

        try
        {
            await using var context = await ContextFactory.CreateDbContextAsync();

            context.RemoveRange(softDeletedMedias);
            await context.SaveChangesAsync();
            
            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred while deleting soft-deleted media: {ex.Message}");
        }
    }
}
