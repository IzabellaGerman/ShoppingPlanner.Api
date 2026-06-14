using ShoppingPlanner.Api.Dtos;

namespace ShoppingPlanner.Api.Services
    {
    public interface IShoppingListService
        {
        Task<IEnumerable<ShoppingListDto>> GetAllAsync();
        Task<ShoppingListDto?> GetByIdAsync(int id);
        Task<ShoppingListDto> CreateAsync(CreateShoppingListDto dto);
        Task<ShoppingListDto?> UpdateAsync(int id, UpdateShoppingListDto dto);
        Task<bool> DeleteAsync(int id);
        }
    }
