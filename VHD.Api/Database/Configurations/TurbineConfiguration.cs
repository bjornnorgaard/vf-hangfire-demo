using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VHD.Api.Database.Models;

namespace VHD.Api.Database.Configurations;

internal class TurbineConfiguration : IEntityTypeConfiguration<Turbine>
{
    public void Configure(EntityTypeBuilder<Turbine> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Unit).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Park).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Disabled).IsRequired();
        builder.Property(t => t.PowerKiloWatts).IsRequired();
        builder.Property(t => t.Efficiency).IsRequired();
        builder.Property(t => t.UptimeSeconds).IsRequired();
        builder.Property(t => t.Result);
        builder.Property(t => t.UpdatedAt);
    }
}
