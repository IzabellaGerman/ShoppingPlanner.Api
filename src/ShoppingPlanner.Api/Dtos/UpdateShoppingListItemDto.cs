namespace ShoppingPlanner.Api.Dtos
    {
    public class UpdateShoppingListItemDto
        {
        public decimal? Quantity { get; set; }
        public bool? IsCompleted { get; set; }
        }
    }
