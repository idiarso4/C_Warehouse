using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).IsRequired().HasConversion<int>();
        builder.Property(u => u.ProfileImagePath).HasMaxLength(200);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);

        builder.HasIndex(u => u.Username).IsUnique().HasDatabaseName("IX_Users_Username");
        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("IX_Users_Email");

        builder.HasMany(u => u.StockMovements).WithOne().HasForeignKey(sm => sm.CreatedBy).HasPrincipalKey(u => u.Username).OnDelete(DeleteBehavior.Restrict);

        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.Role).HasDefaultValue(UserRole.Staff);
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("datetime('now')");
        builder.Property(u => u.UpdatedAt).HasDefaultValueSql("datetime('now')");
        builder.Property(u => u.IsDeleted).HasDefaultValue(false);
    }
}