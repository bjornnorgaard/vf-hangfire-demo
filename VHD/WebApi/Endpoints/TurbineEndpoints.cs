using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Database.Models;

namespace WebApi.Endpoints;

public static class TurbineEndpoints
{
    public static void MapTurbineEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("turbines");

        group.MapGet("", async (WindContext context, CancellationToken ct) =>
        {
            var list = await context.Turbines.ToListAsync(ct);
            return Results.Ok(list);
        });

        group.MapGet("{id:guid}", async (WindContext context, Guid id, CancellationToken ct) =>
        {
            var turbine = await context.Turbines.FirstOrDefaultAsync(t => t.Id == id, ct);
            if (turbine == null) return Results.NotFound();
            return Results.Ok(turbine);
        });

        group.MapPut("{id:guid}", async (WindContext context, Guid id, Turbine turbine, CancellationToken ct) =>
        {
            await context.Turbines
                .Where(t => t.Id == id)
                .ExecuteUpdateAsync(u => u
                        .SetProperty(update => update.Unit, turbine.Unit)
                        .SetProperty(update => update.Park, turbine.Park)
                        .SetProperty(update => update.Disabled, turbine.Disabled)
                        .SetProperty(update => update.PowerKiloWatts, turbine.PowerKiloWatts)
                        .SetProperty(update => update.Efficiency, turbine.Efficiency)
                        .SetProperty(update => update.UptimeSeconds, turbine.UptimeSeconds),
                    ct);
        });

        group.MapGet("stats", async (WindContext context, CancellationToken ct) =>
        {
            var stats = await context.Turbines
                .GroupBy(t => t.Park)
                .Select(g => new
                {
                    Park = g.Key,
                    TurbineCount = g.Count(),
                    PowerKiloWattsSum = g.Sum(t => t.PowerKiloWatts),
                    EfficiencyAverage = g.Average(t => t.Efficiency),
                    UptimeSecondsSum = g.Sum(t => t.UptimeSeconds)
                })
                .ToListAsync(ct);

            return Results.Ok(stats);
        });

        group.MapDelete("/clear", async (WindContext context, CancellationToken ct) =>
        {
            await context.Turbines.ExecuteDeleteAsync(ct);
        });
        
        group.MapGet("seed", async (WindContext context, CancellationToken ct) =>
        {
            var any = await context.Turbines.AnyAsync(cancellationToken: ct);
            if (any) return Results.BadRequest("Already seeded");

            var list = new List<string> { "ABC", "DEF", "GHI", "JKL", "MNO", "PQR", "STU", "VWX" };

            foreach (var park in list)
            {
                for (var n = 0; n < Random.Shared.Next(10, 1000); n++)
                {
                    var turbine = new Turbine
                    {
                        Unit = $"{park}{n:D3}",
                        Park = park,
                        Disabled = false,
                        PowerKiloWatts = Random.Shared.Next(1000),
                        Efficiency = Random.Shared.NextDouble(),
                        UptimeSeconds = Random.Shared.Next(1000000)
                    };
                    await context.Turbines.AddAsync(turbine, ct);
                }
            }

            await context.SaveChangesAsync(ct);
            return Results.Ok();
        });
    }
}