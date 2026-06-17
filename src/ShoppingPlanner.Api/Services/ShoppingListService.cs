using Microsoft.EntityFrameworkCore;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;

namespace ShoppingPlanner.Api.Services
    {
    public class ShoppingListService : IShoppingListService
        {
        private readonly AppDbContext _db;

        public ShoppingListService(AppDbContext db)
            {
            _db = db;
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
                return null;
                }

            list.Name = dto.Name;
            await _db.SaveChangesAsync();

            return MapToDto(list);
            }

        public async Task<bool> DeleteAsync(int id)
            {
            var list = await _db.ShoppingLists.FindAsync(id);
            if (list is null)
                {
                return false;
                }

            _db.ShoppingLists.Remove(list);
            await _db.SaveChangesAsync();
            return true;
            }

        public async Task<ShoppingListItemDto?> AddItemAsync(int listId, CreateShoppingListItemDto dto)
            {
            var list = await _db.ShoppingLists
         .Include(l => l.Items)
         .FirstOrDefaultAsync(l => l.Id == listId);

            if (list is null)
                {
                return null;
                }

            var product = await _db.Products.FindAsync(dto.ProductId);
            if (product is null)
                {
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

            return MapItemToDto(item);
            }

        public async Task<ShoppingListItemDto?> UpdateItemAsync(int listId, int itemId, UpdateShoppingListItemDto dto)
            {
            var item = await _db.ShoppingListItems
                 .Include(i => i.Product)
                 .FirstOrDefaultAsync(l => l.Id == itemId && l.ShoppingListId == listId);

            if (item is null)
                {
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
            return MapItemToDto(item);
            }

        public async Task<bool> RemoveItemAsync(int listId, int itemId)
            {
            var item = await _db.ShoppingListItems
                .FirstOrDefaultAsync(i => i.Id == itemId && i.ShoppingListId == listId);

            if (item is null)
                {
                return false;
                }

            _db.ShoppingListItems.Remove(item);
            await _db.SaveChangesAsync();
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