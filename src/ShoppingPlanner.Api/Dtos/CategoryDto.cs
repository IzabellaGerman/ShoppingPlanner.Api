using System.ComponentModel.DataAnnotations;

namespace ShoppingPlanner.Api.Dtos
    {
    public class CategoryDto
        {
        public int Id { get; set; }
        public required string Name { get; set; }
        
        }
    public class CreateCategoryDto
        {
        [Required]  public required string Name { get; set; }

        }

    public class UpdateCategoryDto
        {
        [Required] public required string Name { get; set; }

        }
    }



