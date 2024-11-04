using Hangfire;
using Microsoft.EntityFrameworkCore;
using VHD.Api.Database;
using VHD.Api.Hangfire;

namespace VHD.Api.Jobs;

[Queue(QueueNames.Controller), AutomaticRetry(Attempts = 3), DisableConcurrentExecution(60)]
public class TurbineController(WindContext context)
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

            RecurringJob.AddOrUpdate<TurbineHandler>(turbine.Unit, job => job.Run(turbine.Id, turbine.Unit, ct), "* * * * *");
        }
    }
}
