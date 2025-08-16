using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Code).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Description).HasMaxLength(500);
        builder.Property(l => l.Zone).HasMaxLength(20);
        builder.Property(l => l.Aisle).HasMaxLength(20);
        builder.Property(l => l.Shelf).HasMaxLength(20);
        builder.Property(l => l.Position).HasMaxLength(20);
        builder.Property(l => l.QRCode).HasMaxLength(100);
        builder.Property(l => l.CapacityUnit).HasMaxLength(20).HasDefaultValue("m³");
        builder.Property(l => l.MaxCapacity).HasColumnType("decimal(10,2)");
        builder.Property(l => l.CurrentCapacity).HasColumnType("decimal(10,2)");
        builder.Property(l => l.MinTemperature).HasColumnType("decimal(5,2)");
        builder.Property(l => l.MaxTemperature).HasColumnType("decimal(5,2)");

        builder.HasIndex(l => l.Code).IsUnique().HasDatabaseName("IX_Locations_Code");
        builder.HasIndex(l => l.QRCode).HasDatabaseName("IX_Locations_QRCode");
        builder.HasIndex(l => l.Zone).HasDatabaseName("IX_Locations_Zone");

        builder.HasMany(l => l.ProductLocations).WithOne(pl => pl.Location).HasForeignKey(pl => pl.LocationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(l => l.StockMovements).WithOne(sm => sm.Location).HasForeignKey(sm => sm.LocationId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(l => l.IsActive).HasDefaultValue(true);
        builder.Property(l => l.CreatedAt).HasDefaultValueSql("datetime('now')");
        builder.Property(l => l.UpdatedAt).HasDefaultValueSql("datetime('now')");
        builder.Property(l => l.IsDeleted).HasDefaultValue(false);
    }
}