using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using ShoppingPlanner.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace ShoppingPlanner.Api.IntegrationTests
    {
    
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
        {
        private readonly PostgreSqlContainer _db = new PostgreSqlBuilder("postgres:16")
    .Build();
        public Task InitializeAsync() => _db.StartAsync();

        async Task IAsyncLifetime.DisposeAsync()
            {
            await _db.DisposeAsync();
            }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
            builder.UseSetting("ConnectionStrings:DefaultConnection", _db.GetConnectionString());

            builder.UseSetting("Jwt:Key", "jhsbvxvkjsdhfjshdgkjhfgkjdhfgjkadbcvjkbcxjvhzkjhvzkjhcvjkzhcxvzkjhcvzkjhv");

            }
        public async Task<HttpClient> CreateAuthenticatedClientAsync()
            {
            var client = CreateClient();
            // уникальный e-mail и пароль
            var email = $"user-{Guid.NewGuid()}@test.com";
            string password = "Test123!";

            // регистрация и логин, как в AuthEndpointTests
            var registrationResponse = await client.PostAsJsonAsync("/api/auth/register", 
                new { Email = email, Password = password });

            registrationResponse.EnsureSuccessStatusCode();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", 
                new { Email = email, Password = password });

            loginResponse.EnsureSuccessStatusCode();

            // прочитать Token из ответа логина
            var body = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                body!.Token);
            return client;
            }
        }
     }
    
