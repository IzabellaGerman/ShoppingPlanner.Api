

namespace ShoppingPlanner.Api.Models
    {
    public class ShoppingListItem
        {
        public int Id {get;set; }
        public int ProductId {get;set; }
        public int ShoppingListId {get;set; }
        public decimal Quantity {get;set; }
        public string? Note {get;set; }
        public bool IsCompleted {get;set; }
        public required Product Product {get;set; }
        public required ShoppingList ShoppingList {get;set; }
        }
    }
