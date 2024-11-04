using Microsoft.EntityFrameworkCore;
using VHD.Api.Database.Models;

namespace VHD.Api.Database;

public class WindContext : DbContext
{
    public WindContext(DbContextOptions<WindContext> options) : base(options)
    {
    }
    
    public DbSet<Turbine> Turbines { get; set; }
}