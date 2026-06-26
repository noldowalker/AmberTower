namespace AuthService.Application;

public sealed class LogoutSessionResult
{
    public bool IsSuccess { get; init; }

    public string ErrorCode { get; init; } = string.Empty;

    public string ErrorMessage { get; init; } = string.Empty;
}
