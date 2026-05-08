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

    public async Task<Result<List<Link>>> GetPendingInvitationsAsync(int userId)
    {
        var result = await Query()
            .Where(x => x.TargetUserId == userId && x.ExpirationDate > DateTime.Now)
            .ToListAsync();
        
        return Result.Ok(result);
        
    }
}
