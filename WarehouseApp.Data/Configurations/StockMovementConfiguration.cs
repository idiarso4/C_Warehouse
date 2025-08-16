using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        // Table name
        builder.ToTable("StockMovements");

        // Primary key
        builder.HasKey(sm => sm.Id);

        // Properties
        builder.Property(sm => sm.MovementType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(sm => sm.Quantity)
            .IsRequired();

        builder.Property(sm => sm.PreviousStock)
            .IsRequired();

        builder.Property(sm => sm.NewStock)
            .IsRequired();

        builder.Property(sm => sm.Notes)
            .HasMaxLength(500);

        builder.Property(sm => sm.Reference)
            .HasMaxLength(100);

        builder.Property(sm => sm.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sm => sm.MovementDate)
            .IsRequired();

        // Indexes
        builder.HasIndex(sm => sm.ProductId)
            .HasDatabaseName("IX_StockMovements_ProductId");

        builder.HasIndex(sm => sm.LocationId)
            .HasDatabaseName("IX_StockMovements_LocationId");

        builder.HasIndex(sm => sm.MovementType)
            .HasDatabaseName("IX_StockMovements_MovementType");

        builder.HasIndex(sm => sm.MovementDate)
            .HasDatabaseName("IX_StockMovements_MovementDate");

        builder.HasIndex(sm => sm.CreatedBy)
            .HasDatabaseName("IX_StockMovements_CreatedBy");

        // Relationships
        builder.HasOne(sm => sm.Product)
            .WithMany(p => p.StockMovements)
            .HasForeignKey(sm => sm.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sm => sm.Location)
            .WithMany(l => l.StockMovements)
            .HasForeignKey(sm => sm.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sm => sm.ToLocation)
            .WithMany()
            .HasForeignKey(sm => sm.ToLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Default values
        builder.Property(sm => sm.MovementDate)
            .HasDefaultValueSql("datetime('now')");

        builder.Property(sm => sm.CreatedAt)
            .HasDefaultValueSql("datetime('now')");

        builder.Property(sm => sm.UpdatedAt)
            .HasDefaultValueSql("datetime('now')");

        builder.Property(sm => sm.IsDeleted)
            .HasDefaultValue(false);
    }
}