using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebApi.Configurations;
using WebApi.Database;

namespace WebApi.Jobs;

[Queue(QueueNames.Scheduler), AutomaticRetry(Attempts = 3)]
public class TurbineScheduler(WindContext context)
{
    public async Task Run(CancellationToken ct)
    {
        var turbines = await context.Turbines.ToListAsync(ct);

        foreach (var turbine in turbines)
        {
            if (turbine.Disabled)
            {
                RecurringJob.RemoveIfExists(turbine.Unit);
                continue;
            }

            RecurringJob.AddOrUpdate<TurbineJob>(turbine.Unit, job => job.Run(turbine.Id, ct), "*/15 * * * *");
        }
    }
}