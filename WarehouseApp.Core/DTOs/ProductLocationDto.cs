namespace WarehouseApp.Core.DTOs;

public class ProductLocationDto
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;

    public int LocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }

    public bool IsPrimaryLocation { get; set; }
    public DateTime LastUpdated { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}