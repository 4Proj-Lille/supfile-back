using Quartz;

namespace SupFile.Back.Api.Extensions;

public static class QuartzExtensions
{
    public static void AddJob<T>(this IServiceCollectionQuartzConfigurator q, ScheduledJobsSettings settings, Microsoft.Extensions.Logging.ILogger logger) where T : class, IJob
    {

        var jobName = typeof(T).Name;

            if (!settings.Jobs.TryGetValue(jobName, out var jobSettings))
        {
            LogHelper.LogInformation(logger, nameof(AddJob), "Job {JobName} not found in settings. Skipping registration.", jobName);
            return;
        }

        if (!jobSettings.IsEnabled)
        {
            LogHelper.LogInformation(logger, nameof(AddJob), "Job {JobName} is disabled in settings. Skipping registration.", jobName);
            return;
        }

        try
        {
            CronExpression.ValidateExpression(jobSettings.CronExpression);
        }
        catch (FormatException ex)
        {
            LogHelper.LogError(logger, nameof(AddJob), ex, "Invalid cron expression for job {JobName}: {CronExpression}. Skipping registration.", jobName, jobSettings.CronExpression);
            return;
        }

        var jobKey = new JobKey(jobName);

        q.AddJob<T>(opts => opts.WithIdentity(jobKey));

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{jobName}-trigger")
            .WithDescription(jobSettings.Description)
            .WithCronSchedule(jobSettings.CronExpression)
        );
    }
}
