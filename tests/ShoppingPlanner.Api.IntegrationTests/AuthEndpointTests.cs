using ShoppingPlanner.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingPlanner.Api.IntegrationTests
    {
    public class AuthEndpointTests: IClassFixture<CustomWebApplicationFactory>
        {
        private readonly CustomWebApplicationFactory _factory;

        public AuthEndpointTests(CustomWebApplicationFactory factory)
            { 
            _factory = factory;
            }

        [Fact]
        public async Task Login_WithRegisteredUser_ReturnsToken()
            {
            //Arrange
            var client = _factory.CreateClient();
            var email = $"user-{Guid.NewGuid()}@test.com";
            var password = "Test123!";

            //Act
            var registerResponse = await client.PostAsJsonAsync("/api/auth/register"
                , new { Email = email, Password = password });

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login"
                , new { Email = email, Password = password });

            //Assert
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            var body = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.NotNull(body);
            Assert.False(string.IsNullOrWhiteSpace(body.Token));

            }
        }
    }
