namespace ApiGateway.Auth;

public sealed class LogoutHttpRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
