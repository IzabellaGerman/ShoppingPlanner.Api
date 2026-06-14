namespace ShoppingPlanner.Api.Dtos
    {
    public class CreateShoppingListDto
        {
        public required string Name { get; set; }
        public List<CreateShoppingListItemDto> Items { get; set; } = new();
        }
    }