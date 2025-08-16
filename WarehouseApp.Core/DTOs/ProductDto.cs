using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Core.DTOs;

public class ProductDto
{
    public int Id { get; set; }

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
    public string CategoryName { get; set; } = string.Empty;

    public int? SupplierId { get; set; }
    public string? SupplierName { get; set; }

    public decimal Price { get; set; }
    public decimal Cost { get; set; }

    public int MinimumStock { get; set; }
    public int CurrentStock { get; set; }

    [StringLength(50)]
    public string Unit { get; set; } = "pcs";

    public string? ImagePath { get; set; }
    public bool IsActive { get; set; }

    public DateTime? ExpiryDate { get; set; }
    public bool HasExpiry { get; set; }

    public decimal? Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateProductDto
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
    public int? SupplierId { get; set; }

    public decimal Price { get; set; }
    public decimal Cost { get; set; }

    public int MinimumStock { get; set; } = 0;

    [StringLength(50)]
    public string Unit { get; set; } = "pcs";

    public DateTime? ExpiryDate { get; set; }
    public bool HasExpiry { get; set; } = false;

    public decimal? Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
}

public class UpdateProductDto : CreateProductDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
}