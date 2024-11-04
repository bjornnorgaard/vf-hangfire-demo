using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Hangfire;

namespace WebApi.Jobs;

[Queue(QueueNames.Scheduler), AutomaticRetry(Attempts = 3), DisableConcurrentExecution(60)]
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

            RecurringJob.AddOrUpdate<TurbineJob>(turbine.Unit, job => job.Run(turbine.Id, ct), "* * * * *");
        }
    }
}
