using Hangfire.Dashboard;

namespace WebApi.Configurations;

public class AnonymousAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}