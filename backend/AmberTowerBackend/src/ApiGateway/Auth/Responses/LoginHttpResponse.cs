namespace ApiGateway.Auth.Responses;

public sealed class LoginHttpResponse
{
    public LoginHttpResponse(
        string userId,
        string email,
        string accessToken,
        long accessTokenExpiresAtUnixSeconds,
        string refreshToken,
        long refreshTokenExpiresAtUnixSeconds)
    {
        UserId = userId;
        Email = email;
        AccessToken = accessToken;
        AccessTokenExpiresAtUnixSeconds = accessTokenExpiresAtUnixSeconds;
        RefreshToken = refreshToken;
        RefreshTokenExpiresAtUnixSeconds = refreshTokenExpiresAtUnixSeconds;
    }

    public string UserId { get; }

    public string Email { get; }

    public string AccessToken { get; }

    public long AccessTokenExpiresAtUnixSeconds { get; }

    public string RefreshToken { get; }

    public long RefreshTokenExpiresAtUnixSeconds { get; }
}
