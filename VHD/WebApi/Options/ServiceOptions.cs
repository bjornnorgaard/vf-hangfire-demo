namespace WebApi.Options;

public class ServiceOptions : AbstractOptions
{
    public string ServiceName { get; set; }
    public string HangfireConnection { get; set; }
    public string DatabaseConnection { get; set; }
    public string HangfireQueues { get; set; }
    public string TelemetryCollectorHost { get; set; }

    public ServiceOptions(IConfiguration configuration) : base(configuration)
    {
    }
}
