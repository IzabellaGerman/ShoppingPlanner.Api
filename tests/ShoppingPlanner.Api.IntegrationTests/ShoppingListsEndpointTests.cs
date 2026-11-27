
using ShoppingPlanner.Api.Dtos;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingPlanner.Api.IntegrationTests
    {
    public class ShoppingListsEndpointTests : IClassFixture<CustomWebApplicationFactory>
        {

        private readonly CustomWebApplicationFactory _factory;

        public ShoppingListsEndpointTests(CustomWebApplicationFactory factory)
            {
            _factory = factory;
            }

        [Fact]
        public async Task GetAll_AfterCreatingList_ReturnsCreatedList()
            {
            //Arrange
            var client = await _factory.CreateAuthenticatedClientAsync();

            //Act
            var createResponse = await client.PostAsJsonAsync("/api/shoppinglists",
                new CreateShoppingListDto { Name = "Weekend" });

            var created = await createResponse.Content.ReadFromJsonAsync<ShoppingListDto>();          

            var listResponse = await client.GetAsync("/api/shoppinglists");

            var lists = await listResponse.Content.ReadFromJsonAsync<List<ShoppingListDto>>();

            //Assert
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

            Assert.Contains(lists!, l => l.Id == created!.Id && l.Name == "Weekend");
            }

        }
    }
