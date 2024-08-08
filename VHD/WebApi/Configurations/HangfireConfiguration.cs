using Hangfire;
using Hangfire.PostgreSql;
using WebApi.Jobs;
using WebApi.Options;

namespace WebApi.Configurations;

public static class ServiceCollectionExtension
{
    public static void AddHangfire(this WebApplicationBuilder builder)
    {
        var opts = new ServiceOptions(builder.Configuration);

        builder.Services.AddHangfire(c => c
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(o => o.UseNpgsqlConnection(opts.HangfireConnection)));

        var queues = opts.HangfireQueues
            .ToLower()
            .Split(',')
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToArray() ?? [];

        builder.Services.AddHangfireServer(o =>
        {
            o.ServerName = $"{opts.ServiceName} {Guid.NewGuid()}";
            if (queues.Length > 0) o.Queues = queues;
        });
    }

    public static void UseHangfire(this WebApplication app)
    {
        app.UseHangfireDashboard(options: new DashboardOptions
        {
            Authorization = new [] { new AnonymousAuthorizationFilter() }
        });

        RecurringJob.AddOrUpdate<Scheduler>("scheduler", x => x.Run(), Cron.Minutely);
    }
}
