using Microsoft.EntityFrameworkCore;
using WebApi.Database.Models;

namespace WebApi.Database;

public class WindContext : DbContext
{
    public WindContext(DbContextOptions<WindContext> options) : base(options)
    {
    }
    
    public DbSet<Turbine> Turbines { get; set; }
}