using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Core.Models;

public class Category : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(7)]
    public string Color { get; set; } = "#007ACC"; // Hex color for UI

    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(50)]
    public string? Icon { get; set; } // Icon name for UI

    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();

    // Aliases for compatibility
    public int? ParentId => ParentCategoryId;
    public ICollection<Category> Children => SubCategories;
    public Category? Parent => ParentCategory;
}