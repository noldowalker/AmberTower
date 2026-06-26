namespace ApiGateway.Auth.Requests;

public sealed class RefreshHttpRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
