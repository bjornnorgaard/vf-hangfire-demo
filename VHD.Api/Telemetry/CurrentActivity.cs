using System.Diagnostics;

namespace VHD.Api.Telemetry;

public static class CurrentActivity
{
    public static void AddTurbineId(Guid id)
    {
        Activity.Current?.AddTag("turbine.id", id.ToString());
    }

    public static void AddEvent(ActivityEvent activityEvent)
    {
        Activity.Current?.AddEvent(new ActivityEvent(activityEvent.Name));
    }
}