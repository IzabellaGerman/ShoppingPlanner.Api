using System.ComponentModel.DataAnnotations;

namespace ShoppingPlanner.Api.Dtos
    {
    public class CreateProductDto
        {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DefaultUnit { get; set; } = string.Empty;
        }
    }
