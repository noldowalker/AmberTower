var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

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

app.Run();
