using Microsoft.EntityFrameworkCore;
using VHD.Api.Database;
using VHD.Api.Database.Models;

namespace VHD.Api.Endpoints;

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

        group.MapDelete("clear",
            async (WindContext context, CancellationToken ct) => { await context.Turbines.ExecuteDeleteAsync(ct); });

        group.MapGet("status", async (WindContext context, CancellationToken ct) =>
        {
            var oldest = await context.Turbines
                .OrderBy(t => t.UpdatedAt)
                .Take(5)
                .ToListAsync(ct);

            var noResults = await context.Turbines
                .Where(t => t.Result == 0)
                .Take(5)
                .ToListAsync(ct);

            var disabled = await context.Turbines
                .Where(t => t.Disabled)
                .Take(5)
                .ToListAsync(ct);

            return Results.Ok(new
            {
                Oldest = oldest,
                NoResults = noResults,
                Disabled = disabled,
            });
        });

        group.MapGet("seed", async (WindContext context, CancellationToken ct) =>
        {
            var any = await context.Turbines.AnyAsync(cancellationToken: ct);
            if (any) return Results.BadRequest("Already seeded");

            var list = new List<string> { "A", "B" };

            foreach (var park in list)
            {
                for (var n = 1; n <= 2; n++)
                {
                    var turbine = new Turbine
                    {
                        Unit = $"{park}{n}",
                        Park = park,
                        Disabled = false,
                        PowerKiloWatts = Random.Shared.Next(1, 1000),
                        Efficiency = Random.Shared.NextDouble(),
                        UptimeSeconds = 10,
                    };
                    await context.Turbines.AddAsync(turbine, ct);
                }
            }

            await context.SaveChangesAsync(ct);
            return Results.Ok();
        });
    }
}