using ShoppingPlanner.Api.Dtos;

namespace ShoppingPlanner.Api.Services
    {
    public interface IProductService
        {
        IEnumerable<ProductDto> GetAll();
        ProductDto? GetById(int id);
        ProductDto Create(CreateProductDto dto);
        ProductDto? Update(int id, UpdateProductDto dto);
        bool Delete(int id);

        }
    }
