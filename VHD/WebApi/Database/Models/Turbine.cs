namespace WebApi.Database.Models;

public class Turbine
{
    public Guid Id { get; set; }
    public string Unit { get; set; }
    public string Park { get; set; }
    public bool Disabled { get; set; }
    public int PowerKiloWatts { get; set; }
    public double Efficiency { get; set; }
    public int UptimeSeconds { get; set; }
}