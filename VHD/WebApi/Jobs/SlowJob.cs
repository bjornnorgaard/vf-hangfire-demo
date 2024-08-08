using Hangfire;
using WebApi.Configurations;

namespace WebApi.Jobs;

public class SlowJob : BaseJob
{
    public SlowJob(ILogger<SlowJob> logger) : base(logger)
    {
    }

    [Queue(JobQueues.Two)]
    protected override Task DoWork()
    {
        Thread.Sleep(Random.Shared.Next(1000, 10_000));
        return Task.CompletedTask;
    }
}
