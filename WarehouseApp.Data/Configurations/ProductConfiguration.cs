using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table name
        builder.ToTable("Products");

        // Primary key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Barcode)
            .HasMaxLength(100);

        builder.Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Cost)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Unit)
            .HasMaxLength(50)
            .HasDefaultValue("pcs");

        builder.Property(p => p.ImagePath)
            .HasMaxLength(200);

        builder.Property(p => p.Weight)
            .HasColumnType("decimal(10,3)");

        builder.Property(p => p.Length)
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.Width)
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.Height)
            .HasColumnType("decimal(10,2)");

        // Indexes
        builder.HasIndex(p => p.SKU)
            .IsUnique()
            .HasDatabaseName("IX_Products_SKU");

        builder.HasIndex(p => p.Barcode)
            .HasDatabaseName("IX_Products_Barcode");

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Products_Name");

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");

        builder.HasIndex(p => p.SupplierId)
            .HasDatabaseName("IX_Products_SupplierId");

        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(p => p.StockMovements)
            .WithOne(sm => sm.Product)
            .HasForeignKey(sm => sm.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.ProductLocations)
            .WithOne(pl => pl.Product)
            .HasForeignKey(pl => pl.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Default values
        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.Property(p => p.HasExpiry)
            .HasDefaultValue(false);

        builder.Property(p => p.MinimumStock)
            .HasDefaultValue(0);

        builder.Property(p => p.CurrentStock)
            .HasDefaultValue(0);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("datetime('now')");

        builder.Property(p => p.UpdatedAt)
            .HasDefaultValueSql("datetime('now')");

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);
    }
}