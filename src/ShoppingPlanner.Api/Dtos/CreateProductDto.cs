using System.ComponentModel.DataAnnotations;

namespace ShoppingPlanner.Api.Dtos;

public class CreateProductDto
    {
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid category.")]
    public int CategoryId { get; set; }

    [Required]
    public required string DefaultUnit { get; set; }
    }