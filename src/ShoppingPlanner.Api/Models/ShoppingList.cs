namespace ShoppingPlanner.Api.Models
    {
    public class ShoppingList
        {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
        }
    }
