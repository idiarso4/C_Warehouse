using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Table name
        builder.ToTable("Categories");

        // Primary key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.Color)
            .HasMaxLength(7)
            .HasDefaultValue("#007ACC");

        builder.Property(c => c.Icon)
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Categories_Name");

        builder.HasIndex(c => c.ParentCategoryId)
            .HasDatabaseName("IX_Categories_ParentCategoryId");

        builder.HasIndex(c => c.SortOrder)
            .HasDatabaseName("IX_Categories_SortOrder");

        // Self-referencing relationship
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship with Products
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Default values
        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.Property(c => c.SortOrder)
            .HasDefaultValue(0);

        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("datetime('now')");

        builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("datetime('now')");

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);
    }
}