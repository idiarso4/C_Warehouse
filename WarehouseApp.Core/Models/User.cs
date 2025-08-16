using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Core.Models;

public enum UserRole
{
    Viewer = 1,
    Staff = 2,
    Manager = 3,
    Admin = 4
}

public class User : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Staff;

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    [StringLength(200)]
    public string? ProfileImagePath { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public string Name => FullName; // Alias for compatibility

    // Navigation properties
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}