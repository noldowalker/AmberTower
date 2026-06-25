namespace AuthService.Application;

public sealed class LoginUserResult
{
    public bool IsSuccess { get; init; }

    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string AccessToken { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }

    public string ErrorCode { get; init; } = string.Empty;

    public string ErrorMessage { get; init; } = string.Empty;
}
