using System.ComponentModel.DataAnnotations;
using WarehouseApp.Core.Models;

namespace WarehouseApp.Core.DTOs;

public class StockMovementDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;

    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;

    public MovementType MovementType { get; set; }
    public string MovementTypeName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }

    public string? Notes { get; set; }
    public string? Reference { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime MovementDate { get; set; }

    // For transfers
    public int? ToLocationId { get; set; }
    public string? ToLocationName { get; set; }
    public string? ToLocationCode { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateStockMovementDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public MovementType MovementType { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(100)]
    public string? Reference { get; set; }

    // For transfers
    public int? ToLocationId { get; set; }
}