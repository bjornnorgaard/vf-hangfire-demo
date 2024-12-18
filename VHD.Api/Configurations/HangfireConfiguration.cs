﻿using Hangfire;
using Hangfire.PostgreSql;
using VHD.Api.Hangfire;
using VHD.Api.Jobs;
using VHD.Api.Options;

namespace VHD.Api.Configurations;

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
            .Select(s => s.Trim())
            .ToArray() ?? [];

        if (queues.Length > 0)
        {
            builder.Services.AddHangfireServer(o =>
            {
                o.ServerName = opts.ServiceName;
                o.WorkerCount = Environment.ProcessorCount;
                o.Queues = queues;
            });
        }
    }

    public static void UseHangfire(this WebApplication app)
    {
        app.UseHangfireDashboard(options: new DashboardOptions
            { Authorization = new[] { new AnonymousAuthorizationFilter() } });

        RecurringJob.AddOrUpdate<TurbineController>("turbine-scheduler",
            x => x.Run(default), "* * * * *");
    }
}