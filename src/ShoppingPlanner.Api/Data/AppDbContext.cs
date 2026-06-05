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
    }