namespace AuthService.Application;

public sealed class RefreshTokenIssueResult
{
    public string Token { get; init; } = string.Empty;

    public string TokenHash { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }
}
