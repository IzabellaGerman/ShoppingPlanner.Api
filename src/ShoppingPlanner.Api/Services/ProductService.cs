using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Services
    {
    public class ProductService : IProductService
        {

        private readonly List<Product> _products = new();
        private int _nextId = 1;

        public IEnumerable<ProductDto> GetAll()
            {
            return _products.Select(MapToDto);
            }

        public ProductDto? GetById(int id)
            {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return product is null ? null : MapToDto(product);
            }

        public ProductDto Create(CreateProductDto dto)
            {
            var product = new Product
                {
                Id = _nextId++,
                Name = dto.Name,
                Category = dto.Category,
                DefaultUnit = dto.DefaultUnit,
                CreatedAt = DateTime.UtcNow
                };

            _products.Add(product);
            return MapToDto(product);
            }

        public ProductDto? Update(int id, UpdateProductDto dto)
            {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
                return null;

            product.Name = dto.Name;
            product.Category = dto.Category;
            product.DefaultUnit = dto.DefaultUnit;

            return MapToDto(product);
            }

        public bool Delete(int id)
            {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product is null)
                return false;

            _products.Remove(product);
            return true;
            }

        private static ProductDto MapToDto(Product product) => new()
            {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            DefaultUnit = product.DefaultUnit,
            CreatedAt = product.CreatedAt
            };
        }
    }
