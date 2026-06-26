using ApiGateway.Auth;
using ApiGateway.CurrentUser;
using ApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthGrpcClient(builder.Configuration);
builder.Services.AddGatewayAuthentication(builder.Configuration);
builder.Services.AddGatewaySwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

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
app.MapCurrentUserEndpoints();

app.Run();
