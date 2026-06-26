namespace ApiGateway.Profile.Requests;

public sealed class UpdateMyProfileHttpRequest
{
    public string Nickname { get; init; } = string.Empty;
}
