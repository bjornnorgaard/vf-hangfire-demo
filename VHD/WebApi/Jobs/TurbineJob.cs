using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebApi.Configurations;
using WebApi.Database;
using WebApi.Telemetry;

namespace WebApi.Jobs;

[Queue(QueueNames.Turbine)]
public class TurbineJob(WindContext context, ILogger<TurbineJob> logger)
{
    public async Task Run(Guid turbineId, CancellationToken ct)
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

        var result = turbine.PowerKiloWatts * turbine.Efficiency * turbine.UptimeSeconds;
        await Task.Delay(Random.Shared.Next(3_000), ct);

        logger.LogInformation("Turbine {TurbineId} produced {Result}", turbine.Id, result);
    }
}