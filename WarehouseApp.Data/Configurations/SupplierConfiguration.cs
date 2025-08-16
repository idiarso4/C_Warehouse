using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.ContactPerson).HasMaxLength(100);
        builder.Property(s => s.Email).HasMaxLength(100);
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.City).HasMaxLength(100);
        builder.Property(s => s.PostalCode).HasMaxLength(20);
        builder.Property(s => s.Country).HasMaxLength(100);
        builder.Property(s => s.TaxNumber).HasMaxLength(50);
        builder.Property(s => s.Notes).HasMaxLength(500);

        builder.HasIndex(s => s.Name).HasDatabaseName("IX_Suppliers_Name");
        builder.HasIndex(s => s.Email).HasDatabaseName("IX_Suppliers_Email");

        builder.HasMany(s => s.Products).WithOne(p => p.Supplier).HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.SetNull);

        builder.Property(s => s.IsActive).HasDefaultValue(true);
        builder.Property(s => s.CreatedAt).HasDefaultValueSql("datetime('now')");
        builder.Property(s => s.UpdatedAt).HasDefaultValueSql("datetime('now')");
        builder.Property(s => s.IsDeleted).HasDefaultValue(false);
    }
}