using Gateway.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Extensions;

public static class ServiceClientsExtenstions
{
    public static void AddServiceClients(this IServiceCollection services)
    {
        services
            .AddSingleton<OrderServiceClient>();

        services
            .AddSingleton<ProductServiceClient>();
    }
}