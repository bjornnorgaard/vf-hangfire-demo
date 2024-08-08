using Hangfire;
using WebApi.Configurations;

namespace WebApi.Jobs;

public class StableJob : BaseJob
{
    public StableJob(ILogger<StableJob> logger) : base(logger)
    {
    }

    [Queue(JobQueues.One)]
    protected override Task DoWork()
    {
        return Task.CompletedTask;
    }
}
