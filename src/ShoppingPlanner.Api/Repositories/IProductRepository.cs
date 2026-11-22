using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Repositories
    {
    public interface IProductRepository
        {
        Task<Product?> GetByIdAsync(int id);
        }
    }
