namespace WebApi.Jobs;

public class FailingJob : BaseJob
{
    public FailingJob(ILogger<FailingJob> logger) : base(logger)
    {
    }

    protected override Task DoWork()
    {
        // return Task.CompletedTask;
        throw new Exception("FailingJob failed");
    }
}
