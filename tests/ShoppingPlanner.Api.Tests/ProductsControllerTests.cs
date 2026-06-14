using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingPlanner.Api.Controllers;
using ShoppingPlanner.Api.Dtos;
using ShoppingPlanner.Api.Services;

namespace ShoppingPlanner.Api.Tests;

public class ProductsControllerTests
    {
    private readonly Mock<IProductService> _mockService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
        {
        _mockService = new Mock<IProductService>();
        var logger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockService.Object, logger.Object);
        }

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
        {
        // Arrange
        var products = new List<ProductDto>
    {
        new ProductDto { Id = 1, Name = "Milk", DefaultUnit = "l" },
        new ProductDto { Id = 2, Name = "Bread", DefaultUnit = "pcs" }
    };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, returned.Count());
        }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkWithProduct()
        {
        // Arrange
        var product = new ProductDto { Id = 1, Name = "Milk", DefaultUnit = "l" };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(1, returned.Id);
        }

    [Fact]
    public async Task GetById_NonExistingId_ReturnsNotFound()
        {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(9999)).ReturnsAsync((ProductDto?)null);

        // Act
        var result = await _controller.GetById(9999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        }
    }