using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Hangfire;
using WebApi.Telemetry;

// ReSharper disable ClassNeverInstantiated.Global

namespace WebApi.Jobs;

[Queue(QueueNames.Turbine)]
public class TurbineHandler(WindContext context, ILogger<TurbineHandler> logger)
{
    [JobDisplayName("TurbineHandler({1})"),
     DisableConcurrentExecution("{0}", 0),
     AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task Run(Guid turbineId, string unit, CancellationToken ct)
    {
        CurrentActivity.AddTurbineId(turbineId);

        var turbine = await context.Turbines.FirstOrDefaultAsync(t => t.Id == turbineId, ct);

        if (turbine == null)
        {
            logger.LogWarning("Turbine {TurbineId} not found", turbineId);
            return;
        }

        if (turbine.Disabled)
        {
            logger.LogWarning("Turbine {TurbineId} is disabled", turbineId);
            return;
        }

        for (var i = 0; i < turbine.UptimeSeconds; i++)
        {
            await Task.Delay(1000, ct);
            logger.LogInformation("Working on {Unit} for {Seconds} seconds", unit, i);

            if (Random.Shared.Next(1000) == 0)
            {
                throw new Exception("This is a transient failure");
            }
        }

        turbine.Result = turbine.PowerKiloWatts * turbine.Efficiency * turbine.UptimeSeconds;
        turbine.UpdatedAt = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync(ct);

        logger.LogInformation("Turbine {TurbineId} produced {Result:F2} DKK", turbine.Id, turbine.Result);
    }
}
