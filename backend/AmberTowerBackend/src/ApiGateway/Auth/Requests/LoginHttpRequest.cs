namespace ApiGateway.Auth.Requests;

public sealed class LoginHttpRequest
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
