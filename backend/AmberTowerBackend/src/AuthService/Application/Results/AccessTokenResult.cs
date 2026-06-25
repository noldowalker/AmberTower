namespace AuthService.Application;

public sealed class AccessTokenResult
{
    public required string Token { get; init; }

    public required DateTime ExpiresAtUtc { get; init; }
}
