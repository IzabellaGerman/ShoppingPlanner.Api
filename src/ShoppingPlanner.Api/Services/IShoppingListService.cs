using ShoppingPlanner.Api.Dtos;

namespace ShoppingPlanner.Api.Services
    {
    public interface IShoppingListService
        {
        Task<IEnumerable<ShoppingListDto>> GetAllAsync(string userId);
        Task<ShoppingListDto?> GetByIdAsync(int id,string userId);
        Task<ShoppingListDto> CreateAsync(CreateShoppingListDto dto, string userId);
        Task<ShoppingListDto?> UpdateAsync(int id, UpdateShoppingListDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<ShoppingListItemDto?> AddItemAsync(int listId, CreateShoppingListItemDto dto, string userId);
        Task<ShoppingListItemDto?> UpdateItemAsync(int listId, int itemId, UpdateShoppingListItemDto dto, string userId);
        Task<bool> RemoveItemAsync(int listId, int itemId, string userId);
        }
    }
