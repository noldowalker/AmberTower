namespace ApiGateway.Auth.Responses;

public sealed class RegisterHttpResponse
{
    public RegisterHttpResponse(string userId, string email)
    {
        UserId = userId;
        Email = email;
    }

    public string UserId { get; }

    public string Email { get; }
}
