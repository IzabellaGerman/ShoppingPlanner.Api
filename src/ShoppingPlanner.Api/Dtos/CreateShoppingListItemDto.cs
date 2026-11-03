using System.ComponentModel.DataAnnotations;

namespace ShoppingPlanner.Api.Dtos
    {
    public class CreateShoppingListItemDto
        {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(0.01, 1000)]
        public decimal Quantity { get; set; }

        [MaxLength(200)]
        public string? Note { get; set; }
        }
    }