namespace SupFile.Back.Data.Repositories;

public class ShareRepository : BaseRepository<Share, int, SupFileContext>, IShareRepository
{
    public ShareRepository(ILogger<ShareRepository> logger, IDbContextFactory<SupFileContext> context) : base(
        logger, context)
    {
    }

}
