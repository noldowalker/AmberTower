using AuthService.Application;
using AuthService.Grpc;
using AuthService.Infrastructure;
using AuthService.Options;
using AuthService.Persistence;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var httpPort = builder.Configuration.GetValue<int?>("Ports:Http") ?? 5080;
var grpcPort = builder.Configuration.GetValue<int?>("Ports:Grpc") ?? 5081;

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(httpPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    options.ListenAnyIP(grpcPort, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpc();
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(AuthDbContext.ConnectionStringName)));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<RefreshTokenOptions>(builder.Configuration.GetSection(RefreshTokenOptions.SectionName));

builder.Services.AddScoped<IAuthUserRepository, AuthUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IRefreshTokenService, SecureRefreshTokenService>();
builder.Services.AddScoped<AuthApplicationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.Migrate();
}

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

app.Run();
