namespace ShoppingPlanner.Api.Dtos
    {
    public class ProductDto
        {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category {  get; set; } = string.Empty;
        public string DefaultUnit {  get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        }
    }
