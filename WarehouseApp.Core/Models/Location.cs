using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Core.Models;

public class Location : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty; // e.g., "A1-B2-C3"

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(20)]
    public string Zone { get; set; } = string.Empty; // e.g., "A", "B", "C"

    [StringLength(20)]
    public string Aisle { get; set; } = string.Empty; // e.g., "1", "2", "3"

    [StringLength(20)]
    public string Shelf { get; set; } = string.Empty; // e.g., "A", "B", "C"

    [StringLength(20)]
    public string Position { get; set; } = string.Empty; // e.g., "1", "2", "3"

    public bool IsActive { get; set; } = true;

    // Hierarchical structure
    public int? ParentId { get; set; }
    public Location? Parent { get; set; }

    [StringLength(100)]
    public string? QRCode { get; set; } // QR code for location identification

    // Capacity management
    public decimal? MaxCapacity { get; set; }
    public decimal? CurrentCapacity { get; set; }

    [StringLength(20)]
    public string? CapacityUnit { get; set; } = "m³";

    // Temperature control for special storage
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }

    // Navigation properties
    public ICollection<Location> Children { get; set; } = new List<Location>();
    public ICollection<ProductLocation> ProductLocations { get; set; } = new List<ProductLocation>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}