using AmberTower.Auth.Contracts;
using ApiGateway.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClient<AuthGrpc.AuthGrpcClient>(options =>
{
    var authServiceUrl = builder.Configuration["Grpc:AuthServiceUrl"] ?? "http://localhost:5081";
    options.Address = new Uri(authServiceUrl);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.MapAuthEndpoints();

app.Run();
