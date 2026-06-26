namespace ApiGateway.CurrentUser.Responses;

public sealed class CurrentUserHttpResponse
{
    public CurrentUserHttpResponse(string userId, string email)
    {
        UserId = userId;
        Email = email;
    }

    public string UserId { get; }

    public string Email { get; }
}
