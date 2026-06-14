using Microsoft.EntityFrameworkCore;
using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Data;

public class AppDbContext : DbContext
    {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Dairy" },
            new Category { Id = 2, Name = "Bakery" },
            new Category { Id = 3, Name = "Vegetables" },
            new Category { Id = 4, Name = "Fruits" },
            new Category { Id = 5, Name = "Beverages" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Milk", CategoryId = 1, DefaultUnit = "l" },
            new Product { Id = 2, Name = "Butter", CategoryId = 1, DefaultUnit = "kg" },
            new Product { Id = 3, Name = "Sourdough Bread", CategoryId = 2, DefaultUnit = "pcs" },
            new Product { Id = 4, Name = "Baguette", CategoryId = 2, DefaultUnit = "pcs" },
            new Product { Id = 5, Name = "Carrots", CategoryId = 3, DefaultUnit = "kg" },
            new Product { Id = 6, Name = "Tomatoes", CategoryId = 3, DefaultUnit = "kg" },
            new Product { Id = 7, Name = "Spinach", CategoryId = 3, DefaultUnit = "kg" },
            new Product { Id = 8, Name = "Apples", CategoryId = 4, DefaultUnit = "kg" },
            new Product { Id = 9, Name = "Bananas", CategoryId = 4, DefaultUnit = "kg" },
            new Product { Id = 10, Name = "Orange Juice", CategoryId = 5, DefaultUnit = "l" }
        );

        modelBuilder.Entity<ShoppingListItem>()
            .HasOne(i => i.ShoppingList)
            .WithMany(l => l.Items)
            .HasForeignKey(i => i.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShoppingListItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }