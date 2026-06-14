namespace ShoppingPlanner.Api.Dtos
    {
    public class CreateShoppingListItemDto
        {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string? Note { get; set; }
        }
    }