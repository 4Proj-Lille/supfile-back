namespace SupFile.Back.Data.Repositories;

public class LinkRepository : BaseRepository<Link, int, SupFileContext>, ILinkRepository
{
    public LinkRepository(ILogger<LinkRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

}
