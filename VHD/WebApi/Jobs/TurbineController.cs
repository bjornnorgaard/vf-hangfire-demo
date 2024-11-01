using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Hangfire;

// ReSharper disable ClassNeverInstantiated.Global

namespace WebApi.Jobs;

[Queue(QueueNames.Scheduler), AutomaticRetry(Attempts = 3), DisableConcurrentExecution(60)]
public class TurbineController(WindContext context, ILogger<TurbineController> logger)
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

            RecurringJob.AddOrUpdate<TurbineHandler>(turbine.Unit, h => h.Run(turbine.Id, turbine.Unit, ct), "* * * * *");
        }
    }
}
