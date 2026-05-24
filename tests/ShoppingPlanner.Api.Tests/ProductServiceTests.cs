using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Tests;

public class ProductServiceTests
    {
    // ===== GetAll =====

    [Fact]
    public void GetAll_WhenNoProducts_ReturnsEmptyCollection()
        {
        // Arrange
        var service = new ProductService();

        // Act
        var result = service.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        }

    [Fact]
    public void GetAll_WhenProductsExist_ReturnsAllProducts()
        {
        // Arrange
        var service = new ProductService();
        service.Create(new CreateProductDto { Name = "Bread", Category = "Bakery", DefaultUnit = "pcs" });
        service.Create(new CreateProductDto { Name = "Milk", Category = "Dairy", DefaultUnit = "l" });

        // Act
        var result = service.GetAll().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        }

    // ===== GetById =====

    [Fact]
    public void GetById_WhenProductExists_ReturnsProduct()
        {
        // Arrange
        var service = new ProductService();
        var created = service.Create(new CreateProductDto { Name = "Bread", Category = "Bakery", DefaultUnit = "pcs" });

        // Act
        var result = service.GetById(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Bread", result.Name);
        }

    [Fact]
    public void GetById_WhenProductDoesNotExist_ReturnsNull()
        {
        // Arrange
        var service = new ProductService();

        // Act
        var result = service.GetById(999);

        // Assert
        Assert.Null(result);
        }

    // ===== Create =====

    [Fact]
    public void Create_WithValidDto_AssignsIncrementalId()
        {
        // Arrange
        var service = new ProductService();

        // Act
        var first = service.Create(new CreateProductDto { Name = "Bread", Category = "Bakery", DefaultUnit = "pcs" });
        var second = service.Create(new CreateProductDto { Name = "Milk", Category = "Dairy", DefaultUnit = "l" });

        // Assert
        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
        }

    [Fact]
    public void Create_WithValidDto_SetsCreatedAtInUtc()
        {
        // Arrange
        var service = new ProductService();
        var before = DateTime.UtcNow;

        // Act
        var result = service.Create(new CreateProductDto { Name = "Bread", Category = "Bakery", DefaultUnit = "pcs" });
        var after = DateTime.UtcNow;

        // Assert
        Assert.Equal(DateTimeKind.Utc, result.CreatedAt.Kind);
        Assert.InRange(result.CreatedAt, before, after);
        }

    // ===== Update =====

    [Fact]
    public void Update_WhenProductExists_UpdatesFields()
        {
        // Arrange
        var service = new ProductService();
        var created = service.Create(new CreateProductDto { Name = "Bread", Category = "Bakery", DefaultUnit = "pcs" });

        // Act
        var updated = service.Update(created.Id, new UpdateProductDto
            {
            Name = "Sourdough Bread",
            Category = "Bakery",
            DefaultUnit = "kg"
            });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Sourdough Bread", updated.Name);
        Assert.Equal("kg", updated.DefaultUnit);
        }

    [Fact]
    public void Update_WhenProductExists_PreservesCreatedAt()
        {
        // Arrange
        var service = new ProductService();
        var created = service.Create(new CreateProductDto { Name = "Bread", Category = "Bakery", DefaultUnit = "pcs" });
        var originalCreatedAt = created.CreatedAt;

        // Act
        var updated = service.Update(created.Id, new UpdateProductDto
            {
            Name = "Updated",
            Category = "Bakery",
            DefaultUnit = "pcs"
            });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(originalCreatedAt, updated.CreatedAt);
        }

    [Fact]
    public void Update_WhenProductDoesNotExist_ReturnsNull()
        {
        // Arrange
        var service = new ProductService();

        // Act
        var result = service.Update(999, new UpdateProductDto
            {
            Name = "Anything",
            Category = "Anything",
            DefaultUnit = "pcs"
            });

        // Assert
        Assert.Null(result);
        }

    // ===== Delete =====

    [Fact]
    public void Delete_WhenProductExists_ReturnsTrueAndRemoves()
        {
        // Arrange
        var service = new ProductService();
        var created = service.Create(new CreateProductDto { Name = "Bread", Category = "Bakery", DefaultUnit = "pcs" });

        // Act
        var deleted = service.Delete(created.Id);
        var afterDelete = service.GetById(created.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(afterDelete);
        }

    [Fact]
    public void Delete_WhenProductDoesNotExist_ReturnsFalse()
        {
        // Arrange
        var service = new ProductService();

        // Act
        var result = service.Delete(999);

        // Assert
        Assert.False(result);
        }
    }