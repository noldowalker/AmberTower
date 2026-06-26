namespace AuthService.Application;

public sealed class RefreshSessionResult
{
    public bool IsSuccess { get; init; }

    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string AccessToken { get; init; } = string.Empty;

    public DateTime AccessTokenExpiresAtUtc { get; init; }

    public string RefreshToken { get; init; } = string.Empty;

    public DateTime RefreshTokenExpiresAtUtc { get; init; }

    public string ErrorCode { get; init; } = string.Empty;

    public string ErrorMessage { get; init; } = string.Empty;
}
