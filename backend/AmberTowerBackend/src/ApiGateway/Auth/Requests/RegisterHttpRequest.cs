namespace ApiGateway.Auth.Requests;

public sealed class RegisterHttpRequest
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}
