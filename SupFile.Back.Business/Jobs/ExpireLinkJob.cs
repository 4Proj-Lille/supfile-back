using System.Resources;
using Quartz;

namespace SupFile.Back.Business.Jobs;

public class ExpireLinkJob : IJob
{
    private readonly ILogger<ExpireLinkJob> _logger;
    private readonly ILinkService _linkService;
    private readonly IUserService _userService;

    public ExpireLinkJob(
        ILogger<ExpireLinkJob> logger,
        ILinkService linkService,
        IUserService userService
        )
    {
        _logger = logger;
        _linkService = linkService;
        _userService = userService;
    }

    public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
    {
        var now = DateTime.Now;

        var links = await _linkService.GetExpiredLinksAsync();

        if (links.IsSuccess && links.Value.Count == 0)
        {
            _logger.LogInformation("{JobName} executed. No unsubmitted expenses found at {Time}", nameof(ExpireLinkJob), now);
            return;
        }
        
        
        foreach (var link in links.Value)
        {
            await _linkService.DeleteAsync(link.Id);
        }
    }
}
