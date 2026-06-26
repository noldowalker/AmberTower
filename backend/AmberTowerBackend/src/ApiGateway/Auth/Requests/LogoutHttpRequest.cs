namespace ApiGateway.Auth.Requests;

public sealed class LogoutHttpRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
