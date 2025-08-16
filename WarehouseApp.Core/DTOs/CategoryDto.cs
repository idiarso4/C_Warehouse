using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Core.DTOs;

public class CategoryDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(7)]
    public string Color { get; set; } = "#007ACC";

    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }

    public bool IsActive { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }

    public int ProductCount { get; set; }
    public List<CategoryDto> SubCategories { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateCategoryDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(7)]
    public string Color { get; set; } = "#007ACC";

    public int? ParentCategoryId { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; } = 0;
}

public class UpdateCategoryDto : CreateCategoryDto
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
}