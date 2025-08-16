using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Core.DTOs;

public class LocationDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public string Zone { get; set; } = string.Empty;
    public string Aisle { get; set; } = string.Empty;
    public string Shelf { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public string? QRCode { get; set; }

    public decimal? MaxCapacity { get; set; }
    public decimal? CurrentCapacity { get; set; }
    public string? CapacityUnit { get; set; }

    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }

    public int ProductCount { get; set; }
    public decimal CapacityUtilization { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateLocationDto
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(20)]
    public string Zone { get; set; } = string.Empty;

    [StringLength(20)]
    public string Aisle { get; set; } = string.Empty;

    [StringLength(20)]
    public string Shelf { get; set; } = string.Empty;

    [StringLength(20)]
    public string Position { get; set; } = string.Empty;

    public decimal? MaxCapacity { get; set; }
    public string? CapacityUnit { get; set; } = "m³";

    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
}

public class UpdateLocationDto : CreateLocationDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
}