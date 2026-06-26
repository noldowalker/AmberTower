using AuthService.Grpc;

namespace AuthService.Extensions;

public static class AuthApplicationEndpointExtensions
{
    public static WebApplication MapAuthApplicationEndpoints(this WebApplication app)
    {
        app.MapGrpcService<AuthGrpcService>();
        app.MapGet("/", () => Results.Ok(new
        {
            service = "AuthService",
            message = "AmberTower authentication internal gRPC service"
        }));
        app.MapGet("/health", () => Results.Ok(new
        {
            status = "ok",
            service = "AuthService",
            utcTime = DateTime.UtcNow
        }));

        return app;
    }
}
