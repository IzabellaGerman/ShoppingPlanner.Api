using Microsoft.EntityFrameworkCore;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Repositories
    {
    public class ShoppingListRepository : IShoppingListRepository
        {
        private readonly AppDbContext _db;

        public ShoppingListRepository(AppDbContext db)
            {
            _db = db;
            }

        public Task<List<ShoppingList>> GetAllAsync()
            {
            return _db.ShoppingLists
                .Include(l => l.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();
            }

        public Task<ShoppingList?> GetByIdAsync(int id)
            {
            return _db.ShoppingLists
                .Include(l => l.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(l => l.Id == id);
            }

        public Task<ShoppingListItem?> GetItemAsync(int listId, int itemId)
            {
            return _db.ShoppingListItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == itemId && i.ShoppingListId == listId);
            }

        public void Add(ShoppingList list) => _db.ShoppingLists.Add(list);

        public void Remove(ShoppingList list) => _db.ShoppingLists.Remove(list);

        public void AddItem(ShoppingListItem item) => _db.ShoppingListItems.Add(item);

        public void RemoveItem(ShoppingListItem item) => _db.ShoppingListItems.Remove(item);

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
        }
    }
