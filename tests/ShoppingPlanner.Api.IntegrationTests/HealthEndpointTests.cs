using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace ShoppingPlanner.Api.IntegrationTests;

public class HealthEndpointTests: IClassFixture<CustomWebApplicationFactory>
    {
    [Fact]
    public async Task GetHealth_WhenAppIsRunning_ReturnOk()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }
    private readonly CustomWebApplicationFactory _factory;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }
    }
