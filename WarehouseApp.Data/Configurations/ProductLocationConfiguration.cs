using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Configurations;

public class ProductLocationConfiguration : IEntityTypeConfiguration<ProductLocation>
{
    public void Configure(EntityTypeBuilder<ProductLocation> builder)
    {
        builder.ToTable("ProductLocations");
        builder.HasKey(pl => pl.Id);

        builder.Property(pl => pl.Quantity).IsRequired();
        builder.Property(pl => pl.MinimumStock).HasDefaultValue(0);
        builder.Property(pl => pl.MaximumStock).HasDefaultValue(0);
        builder.Property(pl => pl.IsPrimaryLocation).HasDefaultValue(false);
        builder.Property(pl => pl.LastUpdated).HasDefaultValueSql("datetime('now')");

        builder.HasIndex(pl => new { pl.ProductId, pl.LocationId }).IsUnique().HasDatabaseName("IX_ProductLocations_ProductId_LocationId");
        builder.HasIndex(pl => pl.ProductId).HasDatabaseName("IX_ProductLocations_ProductId");
        builder.HasIndex(pl => pl.LocationId).HasDatabaseName("IX_ProductLocations_LocationId");

        builder.HasOne(pl => pl.Product).WithMany(p => p.ProductLocations).HasForeignKey(pl => pl.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pl => pl.Location).WithMany(l => l.ProductLocations).HasForeignKey(pl => pl.LocationId).OnDelete(DeleteBehavior.Cascade);

        builder.Property(pl => pl.CreatedAt).HasDefaultValueSql("datetime('now')");
        builder.Property(pl => pl.UpdatedAt).HasDefaultValueSql("datetime('now')");
        builder.Property(pl => pl.IsDeleted).HasDefaultValue(false);
    }
}