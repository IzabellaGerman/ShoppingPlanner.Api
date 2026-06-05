using ShoppingPlanner.Api.Dtos;

namespace ShoppingPlanner.Api.Services
    {
    public interface ICategoryService
        {
        public Task<IEnumerable<CategoryDto>> GetAllAsync();

        public Task<CategoryDto?> GetByIdAsync(int id);


        public Task<CategoryDto> CreateAsync(CreateCategoryDto dto);


        public Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto);


        public Task<bool> DeleteAsync(int id);
           

        }
    }
