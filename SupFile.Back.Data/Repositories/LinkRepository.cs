using SupFile.Back.Core.Errors;

namespace SupFile.Back.Data.Repositories;

public class LinkRepository : BaseRepository<Link, int, SupFileContext>, ILinkRepository
{
    public LinkRepository(ILogger<LinkRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

    public async Task<Result<Link>> GetByTokenAsync(string token)
    {
        var q = Query().Where(x => x.Token == token);
        var entity = await q.FirstOrDefaultAsync();
        if (entity == null)
        {
            return Result.Fail<Link>(new NotFoundError("Invitation not found"));
        }

        return entity;
    }

}
