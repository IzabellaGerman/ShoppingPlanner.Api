namespace ShoppingPlanner.Api.Dtos
    {
    public class ShoppingListItemDto
        {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public string? Note { get; set; }
        public bool IsCompleted { get; set; }
        }

    }
