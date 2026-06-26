namespace ApiGateway.Auth;

public sealed class RefreshHttpRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
