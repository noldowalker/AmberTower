using AmberTower.Profile.Contracts;

namespace ApiGateway.Extensions;

public static class ProfileGatewayServiceCollectionExtensions
{
    public static IServiceCollection AddProfileGrpcClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGrpcClient<ProfileGrpc.ProfileGrpcClient>(options =>
        {
            var profileServiceUrl = configuration["Grpc:ProfileServiceUrl"] ?? "http://localhost:5083";
            options.Address = new Uri(profileServiceUrl);
        });

        return services;
    }
}
