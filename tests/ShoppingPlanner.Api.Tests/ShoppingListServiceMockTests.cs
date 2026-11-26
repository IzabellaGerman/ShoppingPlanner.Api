using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Models;
using ShoppingPlanner.Api.Repositories;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Tests;

// Unit-тесты бизнес-логики ShoppingListService в изоляции:
// репозитории подменены Moq, БД (даже InMemory) не используется.
// Здесь проверяем ЛОГИКУ сервиса (что и в каком порядке он вызывает, что возвращает),
// а работу с EF проверяют ShoppingListServiceTests / ShoppingListServiceItemTests на InMemory.
public class ShoppingListServiceMockTests
    {
    private readonly Mock<IShoppingListRepository> _lists = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly ShoppingListService _service;
    private const string TestUserId = "test-user";
    public ShoppingListServiceMockTests()
        {
        _service = new ShoppingListService(
            _lists.Object,
            _products.Object,
            NullLogger<ShoppingListService>.Instance);
        }

    private static ShoppingList AnyList(int id = 5) => new() { Id = id, Name = "Weekly", UserId = TestUserId };

    private static Product Milk(int id = 1) => new() { Id = id, Name = "Milk", CategoryId = 1, DefaultUnit = "l" };

    // ---- AddItemAsync -------------------------------------------------------

    [Fact]
    public async Task AddItemAsync_ListAndProductExist_ReturnsItemDtoWithProductName()
        {
        // Arrange: репозитории "знают" и список, и товар
        _lists.Setup(r => r.GetByIdAsync(5, TestUserId)).ReturnsAsync(AnyList(5));
        _products.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(Milk(1));
        var dto = new CreateShoppingListItemDto { ProductId = 1, Quantity = 2, Note = "low fat" };

        // Act
        var result = await _service.AddItemAsync(5, dto, TestUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ProductId);
        Assert.Equal("Milk", result.ProductName);
        Assert.Equal(2m, result.Quantity);
        Assert.Equal("low fat", result.Note);
        Assert.False(result.IsCompleted);
        }

    [Fact]
    public async Task AddItemAsync_ListAndProductExist_AddsItemWithGivenArgumentsAndSavesOnce()
        {
        // Arrange
        _lists.Setup(r => r.GetByIdAsync(5, TestUserId)).ReturnsAsync(AnyList(5));
        _products.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(Milk(1));
        var dto = new CreateShoppingListItemDto { ProductId = 1, Quantity = 2, Note = "low fat" };

        // Act
        await _service.AddItemAsync(5, dto, TestUserId);

        // Assert: в репозиторий ушла именно та позиция, и сохранение было ровно одно
        _lists.Verify(r => r.AddItem(It.Is<ShoppingListItem>(i =>
            i.ShoppingListId == 5 &&
            i.ProductId == 1 &&
            i.Quantity == 2m &&
            i.Note == "low fat" &&
            !i.IsCompleted)), Times.Once);
        _lists.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

    [Fact]
    public async Task AddItemAsync_ListNotFound_ReturnsNullAndDoesNotTouchProductsOrSave()
        {
        // Arrange: репозиторий вернул null — списка нет
        _lists.Setup(r => r.GetByIdAsync(99, TestUserId)).ReturnsAsync((ShoppingList?)null);
        var dto = new CreateShoppingListItemDto { ProductId = 1, Quantity = 1 };

        // Act
        var result = await _service.AddItemAsync(99, dto, TestUserId);

        // Assert: NotFound-результат (null) и никаких побочных эффектов
        Assert.Null(result);
        _products.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _lists.Verify(r => r.AddItem(It.IsAny<ShoppingListItem>()), Times.Never);
        _lists.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

    [Fact]
    public async Task AddItemAsync_ProductNotFound_ReturnsNullAndDoesNotAddOrSave()
        {
        // Arrange: список есть, товара нет
        _lists.Setup(r => r.GetByIdAsync(5, TestUserId)).ReturnsAsync(AnyList(5));
        _products.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Product?)null);
        var dto = new CreateShoppingListItemDto { ProductId = 404, Quantity = 1 };

        // Act
        var result = await _service.AddItemAsync(5, dto, TestUserId);

        // Assert
        Assert.Null(result);
        _lists.Verify(r => r.AddItem(It.IsAny<ShoppingListItem>()), Times.Never);
        _lists.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

    // ---- CreateAsync / DeleteAsync -----------------------------------------

    [Fact]
    public async Task CreateAsync_ProductNotFound_ThrowsInvalidOperationExceptionAndDoesNotSave()
        {
        // Arrange
        _products.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Product?)null);
        var dto = new CreateShoppingListDto
            {
            Name = "Party",
            Items = new List<CreateShoppingListItemDto>
                {
                new() { ProductId = 42, Quantity = 1 }
                }
            };

        // Act + Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto, TestUserId));
        Assert.Contains("42", ex.Message);
        _lists.Verify(r => r.Add(It.IsAny<ShoppingList>()), Times.Never);
        _lists.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

    [Fact]
    public async Task DeleteAsync_ListNotFound_ReturnsFalseAndDoesNotRemove()
        {
        // Arrange
        _lists.Setup(r => r.GetByIdAsync(99, TestUserId)).ReturnsAsync((ShoppingList?)null);

        // Act
        var result = await _service.DeleteAsync(99, TestUserId);

        // Assert
        Assert.False(result);
        _lists.Verify(r => r.Remove(It.IsAny<ShoppingList>()), Times.Never);
        _lists.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

    [Fact]
    public async Task CreateAsync_AllProductsExist_ReturnDTO()
        {
        // Arrange
        var dto = new CreateShoppingListDto
            {
            Name = "Party",
            Items = new List<CreateShoppingListItemDto>
                {
                new() { ProductId = 42, Quantity = 1 },
                new() { ProductId = 41, Quantity = 2 }
                }
            };

        _products.Setup(r => r.GetByIdAsync(41)).ReturnsAsync(Milk(41));
        _products.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(Milk(42));

        // Act
        var result = await _service.CreateAsync(dto, TestUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Party", result.Name);
        Assert.Equal(2, result.Items.Count);

        _lists.Verify(r => r.Add(It.Is<ShoppingList>(l =>
            l.Name == "Party" &&
            l.UserId == TestUserId &&
            l.Items.Count == 2)),
            Times.Once);

        _lists.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
