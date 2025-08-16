using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Core.Models;

public class Product : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [StringLength(100)]
    public string Barcode { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public int? LocationId { get; set; }
    public Location? Location { get; set; }

    public decimal Price { get; set; }
    public decimal Cost { get; set; }

    public int MinimumStock { get; set; } = 0;
    public int CurrentStock { get; set; } = 0;

    [StringLength(50)]
    public string Unit { get; set; } = "pcs";

    [StringLength(200)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    // Expiry tracking
    public DateTime? ExpiryDate { get; set; }
    public bool HasExpiry { get; set; } = false;

    // Weight and dimensions
    public decimal? Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }

    // Navigation properties
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<ProductLocation> ProductLocations { get; set; } = new List<ProductLocation>();
}