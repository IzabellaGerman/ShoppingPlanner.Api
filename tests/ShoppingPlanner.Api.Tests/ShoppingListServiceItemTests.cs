using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Tests;

// Логика позиций списка (AddItem / UpdateItem / RemoveItem).
// Реальная БД не нужна: EF Core InMemory, уникальная база на каждый тест.
public class ShoppingListServiceItemTests : IDisposable
    {
    private readonly AppDbContext _db;
    private readonly ShoppingListService _service;

    public ShoppingListServiceItemTests()
        {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);

        // Seed: категория и два продукта — нужны для FK
        _db.Categories.Add(new Category { Id = 1, Name = "Dairy" });
        _db.Products.Add(new Product { Id = 1, Name = "Milk", CategoryId = 1, DefaultUnit = "l" });
        _db.Products.Add(new Product { Id = 2, Name = "Butter", CategoryId = 1, DefaultUnit = "pcs" });
        _db.SaveChanges();

        _service = new ShoppingListService(_db, NullLogger<ShoppingListService>.Instance);
        }

    public void Dispose() => _db.Dispose();

    private Task<ShoppingListDto> CreateEmptyListAsync(string name = "Test list")
        => _service.CreateAsync(new CreateShoppingListDto
            {
            Name = name,
            Items = new List<CreateShoppingListItemDto>()
            });

    [Fact]
    public async Task AddItemAsync_ExistingListAndProduct_ReturnsItemAndSavesIt()
        {
        // Arrange
        var list = await CreateEmptyListAsync();
        var dto = new CreateShoppingListItemDto { ProductId = 1, Quantity = 2, Note = "low fat" };

        // Act
        var result = await _service.AddItemAsync(list.Id, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Milk", result.ProductName);
        Assert.Equal(2m, result.Quantity);
        Assert.Equal("low fat", result.Note);
        Assert.False(result.IsCompleted);
        Assert.Equal(1, await _db.ShoppingListItems.CountAsync(i => i.ShoppingListId == list.Id));
        }

    [Fact]
    public async Task AddItemAsync_NonExistingProduct_ReturnsNullAndSavesNothing()
        {
        // Arrange
        var list = await CreateEmptyListAsync();
        var dto = new CreateShoppingListItemDto { ProductId = 999, Quantity = 1 };

        // Act
        var result = await _service.AddItemAsync(list.Id, dto);

        // Assert
        Assert.Null(result);
        Assert.Empty(_db.ShoppingListItems);
        }

    [Fact]
    public async Task UpdateItemAsync_OnlyIsCompletedSet_KeepsQuantity()
        {
        // Arrange
        var list = await CreateEmptyListAsync();
        var item = await _service.AddItemAsync(list.Id,
            new CreateShoppingListItemDto { ProductId = 1, Quantity = 2 });

        // Act — Quantity = null означает «не менять»
        var result = await _service.UpdateItemAsync(list.Id, item!.Id,
            new UpdateShoppingListItemDto { IsCompleted = true });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
        Assert.Equal(2m, result.Quantity);
        }

    [Fact]
    public async Task UpdateItemAsync_ItemBelongsToAnotherList_ReturnsNullAndKeepsItem()
        {
        // Arrange
        var listA = await CreateEmptyListAsync("A");
        var listB = await CreateEmptyListAsync("B");
        var item = await _service.AddItemAsync(listA.Id,
            new CreateShoppingListItemDto { ProductId = 1, Quantity = 2 });

        // Act — позиция из списка A, но обращаемся через список B
        var result = await _service.UpdateItemAsync(listB.Id, item!.Id,
            new UpdateShoppingListItemDto { Quantity = 5 });

        // Assert
        Assert.Null(result);
        var stored = await _db.ShoppingListItems.SingleAsync(i => i.Id == item.Id);
        Assert.Equal(2m, stored.Quantity);
        }

    [Fact]
    public async Task RemoveItemAsync_ItemBelongsToAnotherList_ReturnsFalseAndKeepsItem()
        {
        // Arrange
        var listA = await CreateEmptyListAsync("A");
        var listB = await CreateEmptyListAsync("B");
        var item = await _service.AddItemAsync(listA.Id,
            new CreateShoppingListItemDto { ProductId = 2, Quantity = 1 });

        // Act
        var removed = await _service.RemoveItemAsync(listB.Id, item!.Id);

        // Assert
        Assert.False(removed);
        Assert.Equal(1, await _db.ShoppingListItems.CountAsync(i => i.Id == item.Id));
        }
    }
