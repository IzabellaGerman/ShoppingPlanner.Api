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
        Task<ShoppingListItemDto?> AddItemAsync(int listId, CreateShoppingListItemDto dto);
        Task<ShoppingListItemDto?> UpdateItemAsync(int listId, int itemId, UpdateShoppingListItemDto dto);
        Task<bool> RemoveItemAsync(int listId, int itemId);
        }
    }
