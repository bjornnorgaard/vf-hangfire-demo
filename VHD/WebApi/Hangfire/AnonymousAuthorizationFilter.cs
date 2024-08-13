using Hangfire.Dashboard;

namespace WebApi.Hangfire;

public class AnonymousAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
