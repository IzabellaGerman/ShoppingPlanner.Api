using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;
using ShoppingPlanner.Api.Repositories;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Tests;

public class ShoppingListServiceTests : IDisposable
    {
    private readonly AppDbContext _db;
    private readonly ShoppingListService _service;

    private const string TestUserId = "test-user";

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

        _service = new ShoppingListService(
            new ShoppingListRepository(_db),
            new ProductRepository(_db),
            NullLogger<ShoppingListService>.Instance);
        }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task GetAllAsync_EmptyDatabase_ReturnsEmptyList()
        {
        // Arrange — база пустая (продуктов нет)

        // Act
        var result = await _service.GetAllAsync(TestUserId);

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

        var result = await _service.CreateAsync(dto, TestUserId);

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
            },
            TestUserId);

        // Act
        var result = await _service.GetByIdAsync(created.Id, TestUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test list", result.Name);
        }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
        // Arrange


        //Act
        var result = await _service.GetByIdAsync(999, TestUserId);

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
            },
            TestUserId);

        //Act
        await  _service.DeleteAsync(created.Id, TestUserId);

        //Assert
        var result = await _service.GetByIdAsync(created.Id, TestUserId);
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
            },
            TestUserId);

        //Act
        var dto = new UpdateShoppingListDto { Name = "New name"};
        created = await _service.UpdateAsync(created.Id, dto, TestUserId);

        //Assert
        Assert.Equal("New name", created.Name);

        }

    }