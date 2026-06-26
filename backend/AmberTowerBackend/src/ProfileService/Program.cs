using ProfileService.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using ProfileService.Application;
using ProfileService.Persistence;

var builder = WebApplication.CreateBuilder(args);

var httpPort = builder.Configuration.GetValue<int?>("Ports:Http") ?? 5082;
var grpcPort = builder.Configuration.GetValue<int?>("Ports:Grpc") ?? 5083;

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
builder.Services.AddDbContext<ProfileDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(ProfileDbContext.ConnectionStringName)));

builder.Services.AddScoped<IPlayerProfileRepository, PlayerProfileRepository>();
builder.Services.AddScoped<ProfileApplicationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();
    dbContext.Database.Migrate();
}

app.MapProfileApplicationEndpoints();

app.Run();
