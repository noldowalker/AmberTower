namespace ProfileService.Application;

public sealed class ProfileResult
{
    public bool IsSuccess { get; init; }

    public Guid PlayerId { get; init; }

    public Guid AuthUserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string Nickname { get; init; } = string.Empty;

    public string ErrorCode { get; init; } = string.Empty;

    public string ErrorMessage { get; init; } = string.Empty;
}
