namespace ApiGateway.Auth.Responses;

public sealed class LogoutHttpResponse
{
    public LogoutHttpResponse(bool success)
    {
        Success = success;
    }

    public bool Success { get; }
}
