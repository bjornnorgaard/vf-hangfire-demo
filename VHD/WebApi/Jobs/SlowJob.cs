using Hangfire;
using WebApi.Configurations;

namespace WebApi.Jobs;

[Queue(JobQueues.Two)]
public class SlowJob : BaseJob
{
    public SlowJob(ILogger<SlowJob> logger) : base(logger)
    {
    }

    protected override Task DoWork()
    {
        Thread.Sleep(Random.Shared.Next(1000, 10_000));
        return Task.CompletedTask;
    }
}
