namespace ShoppingPlanner.Api.Dtos;

public class ProductDto
    {
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public required string DefaultUnit { get; set; }
    public DateTime CreatedAt { get; set; }
    }