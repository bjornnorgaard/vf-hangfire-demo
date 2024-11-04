using Hangfire.Dashboard;

namespace VHD.Api.Hangfire;

public class AnonymousAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
