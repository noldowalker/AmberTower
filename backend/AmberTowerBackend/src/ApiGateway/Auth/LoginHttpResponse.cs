namespace ApiGateway.Auth;

public sealed class LoginHttpResponse
{
    public LoginHttpResponse(string userId, string email, string accessToken, long expiresAtUnixSeconds)
    {
        UserId = userId;
        Email = email;
        AccessToken = accessToken;
        ExpiresAtUnixSeconds = expiresAtUnixSeconds;
    }

    public string UserId { get; }

    public string Email { get; }

    public string AccessToken { get; }

    public long ExpiresAtUnixSeconds { get; }
}
