using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace ShoppingPlanner.Mcp;

public sealed record AuthResponse(string Token, DateTime ExpiresAt);

public sealed record ShoppingListDto(int Id, string Name);

public sealed record ProductDto(int Id, string Name, string? CategoryName, string? DefaultUnit);

public sealed class ShoppingPlannerClient
    {
    private readonly HttpClient _http;
    private readonly TokenStore _tokens;
    private readonly string _email;
    private readonly string _password;

    public ShoppingPlannerClient(HttpClient http, TokenStore tokens, IConfiguration config)
        {
        _http = http;
        _tokens = tokens;
        _email = config["ShoppingPlanner:Email"]
            ?? throw new InvalidOperationException("ShoppingPlanner:Email is not configured");
        _password = config["ShoppingPlanner:Password"]
            ?? throw new InvalidOperationException("ShoppingPlanner:Password is not configured");
        }

    private async Task<(string Token, DateTime ExpiresAtUtc)> LoginAsync(CancellationToken ct)
        {
        var response = await _http.PostAsJsonAsync(
            "api/auth/login", new { email = _email, password = _password }, ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Login failed with status {(int)response.StatusCode}.");

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(ct)
            ?? throw new InvalidOperationException("Login returned an empty body.");

        return (auth.Token, auth.ExpiresAt.ToUniversalTime());
        }

    public async Task<HttpResponseMessage> SendAuthorizedAsync(
        Func<HttpRequestMessage> newRequest, CancellationToken ct)
        {
        var token = await _tokens.GetOrCreateAsync(LoginAsync, ct);
        var response = await SendOnceAsync(newRequest(), token, ct);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        response.Dispose();
        _tokens.Invalidate();
        token = await _tokens.GetOrCreateAsync(LoginAsync, ct);

        return await SendOnceAsync(newRequest(), token, ct);
        }

    private Task<HttpResponseMessage> SendOnceAsync(
        HttpRequestMessage request, string token, CancellationToken ct)
        {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return _http.SendAsync(request, ct);
        }
    public async Task<string> SearchProductsAsync(string query, CancellationToken ct)
        {
        HttpResponseMessage response;
        try
            {
            response = await _http.GetAsync($"api/products?search={Uri.EscapeDataString(query)}", ct);
            }
        catch (HttpRequestException ex)
            {
            return $"The ShoppingPlanner API is not reachable at {_http.BaseAddress}: {ex.Message}";
            }

        if (!response.IsSuccessStatusCode)
            return $"The API returned {(int)response.StatusCode} for a product search.";

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>(cancellationToken: ct)
            ?? new List<ProductDto>();

        if (products.Count == 0)
            return $"No products match '{query}'.";

        const int limit = 10;
        var lines = products.Take(limit).Select(p =>
            $"#{p.Id} {p.Name}{(p.DefaultUnit is null ? "" : $" — {p.DefaultUnit}")}");

        var text = string.Join("\n", lines);

        return products.Count > limit
            ? $"{text}\nShowing {limit} of {products.Count} matches — refine the query."
            : text;
        }

    public async Task<string> GetListsAsync(CancellationToken ct)
        {
        HttpResponseMessage response;
        try
            {
            response = await SendAuthorizedAsync(
                () => new HttpRequestMessage(HttpMethod.Get, "api/shoppinglists"), ct);
            }
        catch (HttpRequestException ex)
            {
            return $"The ShoppingPlanner API is not reachable at {_http.BaseAddress}: {ex.Message}";
            }
        catch (InvalidOperationException ex)
            {
            return $"Authentication against the API failed: {ex.Message}";
            }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return "Authentication failed — check the credentials in user-secrets.";

        if (!response.IsSuccessStatusCode)
            return $"The API returned {(int)response.StatusCode} when listing shopping lists.";

        var lists = await response.Content.ReadFromJsonAsync<List<ShoppingListDto>>(cancellationToken: ct)
            ?? new List<ShoppingListDto>();

        return lists.Count == 0
            ? "No shopping lists found."
            : string.Join("\n", lists.Select(l => $"#{l.Id} {l.Name}"));
        }

    public async Task<string> AddItemAsync(
    int listId, int productId, decimal quantity, string? note, CancellationToken ct)
        {
        if (quantity <= 0)
            return "Quantity must be greater than 0.";

        HttpResponseMessage response;
        try
            {
            response = await SendAuthorizedAsync(() =>
                new HttpRequestMessage(HttpMethod.Post, $"api/shoppinglists/{listId}/items")
                    {
                    Content = JsonContent.Create(new
                        {
                        productId,
                        quantity,
                        note = string.IsNullOrWhiteSpace(note) ? null : note
                        })
                    }, ct);
            }
        catch (HttpRequestException ex)
            {
            return $"The ShoppingPlanner API is not reachable at {_http.BaseAddress}: {ex.Message}";
            }
        catch (InvalidOperationException ex)
            {
            return $"Authentication against the API failed: {ex.Message}";
            }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return $"Could not add the item: list {listId} or product {productId} does not exist. "
                 + "Call get_lists and search_product to verify both ids.";

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return $"The API rejected the item: {await response.Content.ReadAsStringAsync(ct)}";

        if (!response.IsSuccessStatusCode)
            return $"The API returned {(int)response.StatusCode} when adding the item.";

        return $"Added {quantity} of product #{productId} to list #{listId}.";
        }
    }