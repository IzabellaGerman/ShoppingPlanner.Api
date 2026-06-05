namespace ShoppingPlanner.Api.Models;

public class Product
    {
    public int Id { get; set; }
    public required string Name { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }   // navigation property

    public required string DefaultUnit { get; set; }  // kg, l, pcs
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }