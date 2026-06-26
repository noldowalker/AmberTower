using ProfileService.Grpc;

namespace ProfileService.Extensions;

public static class ProfileApplicationEndpointExtensions
{
    public static WebApplication MapProfileApplicationEndpoints(this WebApplication app)
    {
        app.MapGrpcService<ProfileGrpcService>();
        app.MapGet("/", () => Results.Ok(new
        {
            service = "ProfileService",
            message = "AmberTower profile internal gRPC service"
        }));
        app.MapGet("/health", () => Results.Ok(new
        {
            status = "ok",
            service = "ProfileService",
            utcTime = DateTime.UtcNow
        }));

        return app;
    }
}
