namespace AuthService.Application;

public sealed class RegisterUserResult
{
    public bool IsSuccess { get; init; }

    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string ErrorCode { get; init; } = string.Empty;

    public string ErrorMessage { get; init; } = string.Empty;
}
