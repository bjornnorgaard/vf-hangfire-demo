using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using WebApi.Options;

namespace WebApi.Configurations;

public static class TelemetryConfiguration
{
    public static void AddTelemetry(this WebApplicationBuilder builder)
    {
        var serviceOptions = new ServiceOptions(builder.Configuration);

        var serviceName = serviceOptions.ServiceName;
        var otlpHost = new Uri(serviceOptions.TelemetryCollectorHost);

        builder.Logging.AddOpenTelemetry(options => options
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            .AddOtlpExporter(o => o.Endpoint = otlpHost));

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName))
            .WithLogging(logging => logging
                .AddOtlpExporter(o => o.Endpoint = otlpHost), loggerOptions =>
            {
                loggerOptions.IncludeScopes = true;
                loggerOptions.IncludeFormattedMessage = true;
            })
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHangfireInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(o => o.Endpoint = otlpHost))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(o => o.Endpoint = otlpHost));
    }
}
