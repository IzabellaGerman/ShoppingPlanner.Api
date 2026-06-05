using System.ComponentModel.DataAnnotations;

namespace ShoppingPlanner.Api.Dtos;

public class UpdateProductDto
    {
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Required]
    public required string DefaultUnit { get; set; }
    }