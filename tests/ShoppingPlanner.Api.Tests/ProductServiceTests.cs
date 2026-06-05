using Microsoft.EntityFrameworkCore;
using ShoppingPlanner.Api.Data;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Tests;

public class ProductServiceTests : IDisposable
    {
    private readonly AppDbContext _db;
    private readonly ProductService _service;

    public ProductServiceTests()
        {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // уникальная БД на каждый тест
            .Options;

        _db = new AppDbContext(options);

        // Seed категории — нужны для FK constraint
        _db.Categories.Add(new Category { Id = 1, Name = "Vegetables" });
        _db.SaveChanges();

        _service = new ProductService(_db);
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
    public async Task CreateAsync_ValidDto_ReturnsCreatedProduct()
        {
        // Arrange
        var dto = new CreateProductDto
            {
            Name = "Carrot",
            CategoryId = 1,
            DefaultUnit = "kg"
            };

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Carrot", result.Name);
        Assert.Equal("kg", result.DefaultUnit);
        Assert.True(result.Id > 0);
        }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsProduct()
        {
        // Arrange
        var created = await _service.CreateAsync(new CreateProductDto
            {
            Name = "Milk",
            CategoryId = 1,
            DefaultUnit = "l"
            });

        // Act
        var result = await _service.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Milk", result.Name);
        }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
        // Act
        var result = await _service.GetByIdAsync(9999);

        // Assert
        Assert.Null(result);
        }

    [Fact]
    public async Task UpdateAsync_ExistingId_UpdatesAndReturnsProduct()
        {
        // Arrange
        var created = await _service.CreateAsync(new CreateProductDto
            {
            Name = "Old Name",
            CategoryId = 1,
            DefaultUnit = "pcs"
            });
        var originalCreatedAt = created.CreatedAt;

        // Act
        var updated = await _service.UpdateAsync(created.Id, new UpdateProductDto
            {
            Name = "New Name",
            CategoryId = 1,
            DefaultUnit = "kg"
            });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("New Name", updated.Name);
        Assert.Equal("kg", updated.DefaultUnit);
        Assert.Equal(originalCreatedAt, updated.CreatedAt); // CreatedAt не изменился!
        }

    [Fact]
    public async Task DeleteAsync_ExistingId_ReturnsTrueAndRemovesProduct()
        {
        // Arrange
        var created = await _service.CreateAsync(new CreateProductDto
            {
            Name = "Potato",
            CategoryId = 1,
            DefaultUnit = "kg"
            });

        // Act
        var deleted = await _service.DeleteAsync(created.Id);
        var afterDelete = await _service.GetByIdAsync(created.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(afterDelete);
        }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ReturnsFalse()
        {
        // Act
        var result = await _service.DeleteAsync(9999);

        // Assert
        Assert.False(result);
        }
    }