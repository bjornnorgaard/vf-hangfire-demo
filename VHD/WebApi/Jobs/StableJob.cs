namespace WebApi.Jobs;

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