namespace WebApi.Jobs;

public class FlakyJob : BaseJob
{
    public FlakyJob(ILogger<FlakyJob> logger) : base(logger)
    {
    }

    protected override Task DoWork()
    {
        if (Random.Shared.Next(0, 2) == 0)
        {
            throw new Exception("FlakyJob failed");
        }

        return Task.CompletedTask;
    }
}