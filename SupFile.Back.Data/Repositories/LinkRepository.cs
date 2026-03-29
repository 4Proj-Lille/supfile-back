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
        return await FindOneAsync<Link>(x => x.Token == token);
    }
}
