using Hangfire;

namespace WebApi.Jobs;

public class Scheduler : BaseJob
{
    public Scheduler(ILogger<Scheduler> logger) : base(logger)
    {
    }

    protected override Task DoWork()
    {
        for (int i = 0; i < 10; i++)
        {
            BackgroundJob.Enqueue<StableJob>(x => x.Run());
            BackgroundJob.Enqueue<FlakyJob>(x => x.Run());
            BackgroundJob.Enqueue<FailingJob>(x => x.Run());
            BackgroundJob.Enqueue<SlowJob>(x => x.Run());
        }

        return Task.CompletedTask;
    }
}
