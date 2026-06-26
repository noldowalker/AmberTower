namespace ApiGateway.Auth;

public sealed class LogoutHttpResponse
{
    public LogoutHttpResponse(bool success)
    {
        Success = success;
    }

    public bool Success { get; }
}
