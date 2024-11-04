using Hangfire;
using Microsoft.EntityFrameworkCore;
using VHD.Api.Configurations;
using VHD.Api.Database;
using VHD.Api.Endpoints;
using VHD.Api.Options;

var builder = WebApplication.CreateBuilder(args);
builder.AddTelemetry();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = new ServiceOptions(builder.Configuration).DatabaseConnection;
builder.Services.AddDbContext<WindContext>(o => o.UseNpgsql(cs));
builder.AddHangfire();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHangfire();
app.Services.CreateScope().ServiceProvider.GetService<WindContext>()?.Database.Migrate();
app.MapTurbineEndpoints();
app.MapJobEndpoints();

app.MapGet("/jobs", (WindContext context, CancellationToken ct) =>
{
    var servers = JobStorage.Current
        .GetMonitoringApi().Servers()
        .Select(s => new
        {
            s.Name,
            s.Queues
        });

    var queues = JobStorage.Current
        .GetMonitoringApi().Queues()
        .Select(q => new
        {
            q.Name,
            q.Length,
        });

    var jobs = JobStorage.Current
        .GetMonitoringApi()
        .ScheduledJobs(0, 10)
        .Select(j => new
        {
            j.Key,
            Type = j.Value.Job.Type.Name,
            Method = j.Value.Job.Method.Name,
            j.Value.ScheduledAt
        });

    var result = new
    {
        Servers = servers,
        Queues = queues,
        Jobs = jobs,
    };

    return Task.FromResult(Results.Ok(result));
});

app.Run();
