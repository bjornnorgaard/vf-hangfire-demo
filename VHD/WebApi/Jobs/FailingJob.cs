namespace WebApi.Jobs;

public class FailingJob : BaseJob
{
    public FailingJob(ILogger<FailingJob> logger) : base(logger)
    {
    }

    protected override Task DoWork()
    {
        if (Random.Shared.Next(0, 2) == 0)
        {
            return Task.CompletedTask;
        }

        throw new Exception("FailingJob failed");
    }
}
