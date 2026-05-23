namespace ShoppingPlanner.Api.Models;


public class Product
    { 
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required string DefaultUnit { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }