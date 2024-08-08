using Hangfire;
using WebApi.Configurations;

namespace WebApi.Jobs;

[Queue(JobQueues.One)]
public class StableJob : BaseJob
{
    public StableJob(ILogger<StableJob> logger) : base(logger)
    {
    }

    protected override Task DoWork()
    {
        return Task.CompletedTask;
    }
}
