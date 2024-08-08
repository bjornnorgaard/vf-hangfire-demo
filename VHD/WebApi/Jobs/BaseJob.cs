using System.Diagnostics;

namespace WebApi.Jobs;

public abstract class BaseJob
{
    private readonly ILogger<BaseJob> _logger;

    protected BaseJob(ILogger<BaseJob> logger)
    {
        _logger = logger;
    }

    protected abstract Task DoWork();

    public async Task Run()
    {
        var sw = Stopwatch.StartNew();
        var jobType = GetType().Name;
        _logger.LogInformation("{Job} is running", jobType);

        await DoWork();

        var elapsed = sw.ElapsedMilliseconds;
        _logger.LogInformation("{Job} done in {ElapsedMilliseconds}ms", jobType, elapsed);
    }
}
