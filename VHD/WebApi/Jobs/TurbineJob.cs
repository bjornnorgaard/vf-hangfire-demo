using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Hangfire;
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

        if (Random.Shared.Next(100) < 10)
        {
            throw new Exception("This is a transient failure");
        }

        turbine.Result = turbine.PowerKiloWatts * turbine.Efficiency * turbine.UptimeSeconds;
        turbine.UpdatedAt = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync(ct);

        logger.LogInformation("Turbine {TurbineId} produced {Result:F2} DKK", turbine.Id, turbine.Result);
    }
}
