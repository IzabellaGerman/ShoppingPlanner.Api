using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace ShoppingPlanner.Mcp;

public sealed record AuthResponse(string Token, DateTime ExpiresAt);

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
    }