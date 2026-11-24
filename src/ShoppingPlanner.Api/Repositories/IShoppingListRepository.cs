using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Repositories
    {
    /// <summary>
    /// Доступ к спискам покупок и их позициям (позиции — часть агрегата списка).
    /// Add/Remove только помечают изменения, в БД они уходят после SaveChangesAsync.
    /// </summary>
    public interface IShoppingListRepository
        {
        Task<List<ShoppingList>> GetAllAsync(string userId);
        Task<ShoppingList?> GetByIdAsync(int id, string userId);
        Task<ShoppingListItem?> GetItemAsync(int listId, int itemId, string userId);
        void Add(ShoppingList list);
        void Remove(ShoppingList list);
        void AddItem(ShoppingListItem item);
        void RemoveItem(ShoppingListItem item);
        Task SaveChangesAsync();
        }
    }
