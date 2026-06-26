namespace ApiGateway.Extensions;

public static class GatewayApplicationEndpointExtensions
{
    public static WebApplication MapGatewayApplicationEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Ok(new
        {
            service = "ApiGateway",
            message = "AmberTower backend entry point"
        }));

        app.MapGet("/health", () => Results.Ok(new
        {
            status = "ok",
            service = "ApiGateway",
            utcTime = DateTime.UtcNow
        }));

        return app;
    }
}
