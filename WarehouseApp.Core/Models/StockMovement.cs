using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Core.Models;

public enum MovementType
{
    StockIn = 1,
    StockOut = 2,
    Transfer = 3,
    Adjustment = 4,
    Return = 5
}

public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }

    public MovementType MovementType { get; set; }

    public int Quantity { get; set; }

    public int PreviousStock { get; set; }
    public int NewStock { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(100)]
    public string? Reference { get; set; } // PO number, invoice, etc.

    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    // User who performed the movement
    public string? UserId { get; set; }
    public User? User { get; set; }

    public DateTime MovementDate { get; set; } = DateTime.UtcNow;

    // For transfers
    public int? ToLocationId { get; set; }
    public Location? ToLocation { get; set; }
}