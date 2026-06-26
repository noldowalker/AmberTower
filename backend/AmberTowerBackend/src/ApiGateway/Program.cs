using ApiGateway.Auth;
using ApiGateway.CurrentUser;
using ApiGateway.Extensions;
using ApiGateway.Profile;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthGrpcClient(builder.Configuration);
builder.Services.AddProfileGrpcClient(builder.Configuration);
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

app.MapGatewayApplicationEndpoints();
app.MapAuthEndpoints();
app.MapCurrentUserEndpoints();
app.MapProfileEndpoints();

app.Run();
