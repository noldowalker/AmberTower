namespace ApiGateway.Profile.Responses;

public sealed class ProfileHttpResponse
{
    public ProfileHttpResponse(string playerId, string authUserId, string email, string nickname)
    {
        PlayerId = playerId;
        AuthUserId = authUserId;
        Email = email;
        Nickname = nickname;
    }

    public string PlayerId { get; }

    public string AuthUserId { get; }

    public string Email { get; }

    public string Nickname { get; }
}
