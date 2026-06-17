using Microsoft.EntityFrameworkCore;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Services
    {
    public class ShoppingListService : IShoppingListService
        {
        private readonly AppDbContext _db;
        private readonly ILogger<ShoppingListService> _logger;

        public ShoppingListService(AppDbContext db, ILogger<ShoppingListService> logger)
            {
            _db = db;
            _logger = logger;
            }

        public async Task<IEnumerable<ShoppingListDto>> GetAllAsync()
            {
            var lists = await _db.ShoppingLists
                .Include(l => l.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();

            return lists.Select(MapToDto);
            }

        public async Task<ShoppingListDto?> GetByIdAsync(int id)
            {
            var list = await _db.ShoppingLists
                .Include(l => l.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(l => l.Id == id);

            return list is null ? null : MapToDto(list);
            }

        public async Task<ShoppingListDto> CreateAsync(CreateShoppingListDto dto)
            {
            var list = new ShoppingList
                {
                Name = dto.Name,
                Items = new List<ShoppingListItem>()
                };

            foreach (var itemDto in dto.Items)
                {
                var product = await _db.Products.FindAsync(itemDto.ProductId);
                if (product is null)
                    {
                    throw new InvalidOperationException($"Product with id {itemDto.ProductId} not found.");
                    }

                list.Items.Add(new ShoppingListItem
                    {
                    ProductId = itemDto.ProductId,
                    Product = product,
                    Quantity = itemDto.Quantity,
                    Note = itemDto.Note,
                    IsCompleted = false,
                    ShoppingList = list
                    });
                }

            _db.ShoppingLists.Add(list);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Created shopping list {ListId} with {ItemCount} items", list.Id, list.Items.Count);
            // Перезагружаем, чтобы Items.Product точно были подгружены
            return MapToDto(list);
            }

        public async Task<ShoppingListDto?> UpdateAsync(int id, UpdateShoppingListDto dto)
            {
            var list = await _db.ShoppingLists
                .Include(l => l.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (list is null)
                {
                _logger.LogWarning("Shopping list {ListId} not found for update", id);
                return null;
                }

            list.Name = dto.Name;

            await _db.SaveChangesAsync();

            _logger.LogInformation("Updated shopping list {ListId}", list.Id);

            return MapToDto(list);
            }

        public async Task<bool> DeleteAsync(int id)
            {
            var list = await _db.ShoppingLists.FindAsync(id);
            if (list is null)
                {
                _logger.LogWarning("Shopping list {ListId} not found for deletion", id);
                return false;
                }

            _db.ShoppingLists.Remove(list);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Deleted shopping list {ListId}", id);
            return true;
            }

        public async Task<ShoppingListItemDto?> AddItemAsync(int listId, CreateShoppingListItemDto dto)
            {
            var list = await _db.ShoppingLists
         .Include(l => l.Items)
         .FirstOrDefaultAsync(l => l.Id == listId);

            if (list is null)
                {
                _logger.LogWarning("Shopping list {ListId} not found for adding", listId);
                return null;
                }

            var product = await _db.Products.FindAsync(dto.ProductId);
            if (product is null)
                {
                _logger.LogWarning("Product {ProductId} not found for adding", dto.ProductId);
                return null;
                }

            var item = new ShoppingListItem
                {
                ShoppingListId = listId,
                ShoppingList = list,
                ProductId = dto.ProductId,
                Product = product,
                Quantity = dto.Quantity,
                Note = dto.Note,
                IsCompleted = false
                };

            _db.ShoppingListItems.Add(item);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Added item {ItemId} (product {ProductId}) to shopping list {ListId}", item.Id, item.ProductId, listId);
            return MapItemToDto(item);
            }

        public async Task<ShoppingListItemDto?> UpdateItemAsync(int listId, int itemId, UpdateShoppingListItemDto dto)
            {
            var item = await _db.ShoppingListItems
                 .Include(i => i.Product)
                 .FirstOrDefaultAsync(l => l.Id == itemId && l.ShoppingListId == listId);

            if (item is null)
                {
                _logger.LogWarning("Shopping list item {ItemId} not found in list {ListId} for update", itemId, listId);
                return null;
                }

            if (dto.Quantity is not null)
                {
                item.Quantity = dto.Quantity.Value;
                }

            if (dto.IsCompleted is not null)
                {
                item.IsCompleted = dto.IsCompleted.Value;
                }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Updated item {ItemId} in shopping list {ListId}", itemId, listId);
            return MapItemToDto(item);
            }

        public async Task<bool> RemoveItemAsync(int listId, int itemId)
            {
            var item = await _db.ShoppingListItems
                .FirstOrDefaultAsync(i => i.Id == itemId && i.ShoppingListId == listId);

            if (item is null)
                {
                _logger.LogWarning("Shopping list item {ItemId} not found in list {ListId} for deletion", itemId, listId);
                return false;
                }

            _db.ShoppingListItems.Remove(item);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Removed item {ItemId} in shopping list {ListId}", itemId, listId);
            return true;
            }

        private static ShoppingListDto MapToDto(ShoppingList list)
            {
            return new ShoppingListDto
                {
                Id = list.Id,
                Name = list.Name,
                CreatedAt = list.CreatedAt,
                Items = list.Items.Select(i => new ShoppingListItemDto
                    {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Note = i.Note,
                    IsCompleted = i.IsCompleted
                    }).ToList()
                };
            }

        private static ShoppingListItemDto MapItemToDto(ShoppingListItem item)
            {
            
               return new ShoppingListItemDto
                   {                   
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity,
                    Note = item.Note,
                    IsCompleted = item.IsCompleted
                    
                   };
            }
        }    
    }