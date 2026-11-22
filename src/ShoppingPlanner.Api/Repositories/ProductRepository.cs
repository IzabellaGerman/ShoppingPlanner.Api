using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Repositories
    {
    public class ProductRepository : IProductRepository
        {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db)
            {
            _db = db;
            }

        public async Task<Product?> GetByIdAsync(int id) => await _db.Products.FindAsync(id);
        }
    }
