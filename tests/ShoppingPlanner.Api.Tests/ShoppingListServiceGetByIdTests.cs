using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ShoppingPlanner.Api.Models;
using ShoppingPlanner.Api.Repositories;
using ShoppingPlanner.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingPlanner.Api.Tests
    {
    public class ShoppingListServiceGetByIdTests
        {
        private readonly Mock<IShoppingListRepository> _lists = new();
        private readonly Mock<IProductRepository> _products = new();

        private readonly ShoppingListService _service;

        public ShoppingListServiceGetByIdTests()
            {
            _service = new ShoppingListService(_lists.Object, _products.Object,
                NullLogger<ShoppingListService>.Instance);
            }

        [Fact]
        public async Task GetByIdAsync_ListNotFound_ReturnsNull()
            {
            //Arrange
            _lists.Setup(r => r.GetByIdAsync(99)).Returns(Task.FromResult((ShoppingList?)null));

            //Act
            var result = await _service.GetByIdAsync(99);

            //Assert
            Assert.Null(result);


            }

        [Fact]
        public async Task GetByIdAsync_IdPassed_CallsRepositoryOnceWithSameId()
            {
            //Arrange

            //Act
            await _service.GetByIdAsync(7);

            //Assert
            _lists.Verify(r => r.GetByIdAsync(7), Times.Once);

            }

        [Fact]
        public async Task GetByIdAsync_ListFound_ReturnsMappedDTO()
            {

            //Arrange
            var list = new ShoppingList
                {
                Id = 1,
                Name = "Weekly",
                CreatedAt = new DateTime(2026, 1, 15)
                };

            _lists.Setup(r => r.GetByIdAsync(1)).Returns(Task.FromResult<ShoppingList?>(list));

            //Act
            var result = await _service.GetByIdAsync(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(list.Id, result.Id);
            Assert.Empty(result.Items);

            }
        }
    }
