using FluentAssertions;
using Hangfire;
using Hangfire.PostgreSql;
using Testcontainers.PostgreSql;

namespace VHD.Api.Tests;

public class RecurringJobTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _sqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();

    private BackgroundJobServer _server = null!;

    [Fact]
    public async Task AssertSingleInstance()
    {
        const string jobId = "test";

        RecurringJob.AddOrUpdate<TestJob>(jobId, t => t.Run(100), "* * * * *");
        RecurringJob.TriggerJob(jobId);
        RecurringJob.TriggerJob(jobId);
        RecurringJob.TriggerJob(jobId);
        RecurringJob.TriggerJob(jobId);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
        JobStorage.Current.GetMonitoringApi().SucceededListCount().Should().Be(1);

        RecurringJob.TriggerJob(jobId);
        RecurringJob.TriggerJob(jobId);

        await Task.Delay(TimeSpan.FromMilliseconds(500));
        JobStorage.Current.GetMonitoringApi().SucceededListCount().Should().Be(2);
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        var cs = _sqlContainer.GetConnectionString();
        GlobalConfiguration.Configuration.UsePostgreSqlStorage(o => o.UseNpgsqlConnection(cs));
        _server = new BackgroundJobServer();
    }

    public async Task DisposeAsync()
    {
        _server.Dispose();
        await _sqlContainer.DisposeAsync();
    }
}

public class TestJob
{
    [DisableConcurrentExecution(0)]
    public async Task Run(int milliseconds = 0)
    {
        await Task.Delay(milliseconds);
    }
}
