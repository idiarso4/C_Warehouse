namespace WarehouseApp.Core.Models;

public class ProductLocation : BaseEntity
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }

    public int Quantity { get; set; }

    public int MinimumStock { get; set; } = 0;
    public int MaximumStock { get; set; } = 0;

    public bool IsPrimaryLocation { get; set; } = false;

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}