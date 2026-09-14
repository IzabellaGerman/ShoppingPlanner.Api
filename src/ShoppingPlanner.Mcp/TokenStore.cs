namespace ShoppingPlanner.Mcp;

public sealed class TokenStore
    {
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _token;
    private DateTime _expiresAtUtc;

    public async Task<string> GetOrCreateAsync(
        Func<CancellationToken, Task<(string Token, DateTime ExpiresAtUtc)>> login,
        CancellationToken ct)
        {
        if (IsValid()) return _token!;

        await _gate.WaitAsync(ct);
        try
            {
            if (IsValid()) return _token!;

            var result = await login(ct);
            _token = result.Token;
            _expiresAtUtc = result.ExpiresAtUtc;
            return _token;
            }
        finally
            {
            _gate.Release();
            }
        }

    public void Invalidate()
        {
        _token = null;
        _expiresAtUtc = default;
        }

    private bool IsValid() =>
        _token is not null && _expiresAtUtc > DateTime.UtcNow.AddMinutes(1);
    }