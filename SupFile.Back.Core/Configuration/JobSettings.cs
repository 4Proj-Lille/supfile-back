namespace SupFile.Back.Core.Configuration;

public class ScheduledJobsSettings
{
    public Dictionary<string, JobSettings> Jobs { get; set; }
}

public class JobSettings
{
    public string CronExpression { get; set; }
    public string Description { get; set; }
    public bool IsEnabled { get; set; }
}
