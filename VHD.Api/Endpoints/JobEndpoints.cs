using Hangfire;

namespace VHD.Api.Endpoints;

public static class JobEndpoints
{
    public static void MapJobEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("jobs");

        group.MapGet("trigger/{id}", ( string id, CancellationToken ct) =>
        {
            RecurringJob.TriggerJob(id);
            return Results.Ok();
        });
    }
}