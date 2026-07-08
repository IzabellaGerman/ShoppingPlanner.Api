using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Tests;

public class ShoppingListServiceTests : IDisposable
    {
    private readonly AppDbContext _db;
    private readonly ShoppingListService _service;

    public ShoppingListServiceTests()
        {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);

        // Seed категории и продукта — нужны для FK
        _db.Categories.Add(new Category { Id = 1, Name = "Dairy" });
        _db.Products.Add(new Product { Id = 1, Name = "Milk", CategoryId = 1, DefaultUnit = "l" });
        _db.SaveChanges();

        _service = new ShoppingListService(_db, NullLogger<ShoppingListService>.Instance);
        }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
        // Arrange — база пустая (продуктов нет)

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Empty(result);
        }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedList()
        {
        var dto = new CreateShoppingListDto
            {
            Name = "Weekend shopping",
            Items = new List<CreateShoppingListItemDto>
            {
            new CreateShoppingListItemDto { ProductId = 1, Quantity = 2, Note = "fresh" }
            }
            };

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Weekend shopping", result.Name);
        Assert.Single(result.Items);
        Assert.Equal("Milk", result.Items[0].ProductName);
        }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsList()
        {
        // Arrange
        var created = await _service.CreateAsync(new CreateShoppingListDto
            {
            Name = "Test list",
            Items = new List<CreateShoppingListItemDto>()  // пустой список — это ок
            });

        // Act
        var result = await _service.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test list", result.Name);
        }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
        // Arrange


        //Act
        var result = await _service.GetByIdAsync(999);

        //Assert
        Assert.Null(result);

        }

    [Fact]
    public async Task DeleteAsync_ExistingId_RemovesList()
        {
        // Arrange
        var created = await _service.CreateAsync(new CreateShoppingListDto
            {
            Name = "Test list",
            Items = new List<CreateShoppingListItemDto>()
            });

        //Act
        await  _service.DeleteAsync(created.Id);

        //Assert
        var result = await _service.GetByIdAsync(created.Id);
        Assert.Null(result);

        }


    [Fact]
    public async Task UpdateAsync_ExistingId_UpdatesName()
        {
        // Arrange
        var created = await _service.CreateAsync(new CreateShoppingListDto
            {
            Name = "Test list",
            Items = new List<CreateShoppingListItemDto>()
            });

        //Act
        var dto = new UpdateShoppingListDto { Name = "New name"};
        created = await _service.UpdateAsync(created.Id, dto);

        //Assert
        Assert.Equal("New name", created.Name);

        }

    }