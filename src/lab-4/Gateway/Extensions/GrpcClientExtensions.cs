using Gateway.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderServiceClient = Lab3.Contracts.Services.Order.OrderService.OrderServiceClient;
using ProductServiceClient = Lab3.Contracts.Services.Product.ProductService.ProductServiceClient;

namespace Gateway.Extensions;

public static class GrpcClientExtensions
{
    public static void AddMyGrpcClient(this IServiceCollection services)
    {
        services
            .AddGrpcClient<OrderServiceClient>((serviceProvider, optionsFactory) =>
            {
                GrpcServicesOptions options = serviceProvider.GetRequiredService<IOptions<GrpcServicesOptions>>().Value;

                ArgumentNullException.ThrowIfNull(options.OrderService);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.OrderService.Address);

                optionsFactory.Address = new Uri(options.OrderService.Address);
            });

        services
            .AddGrpcClient<ProductServiceClient>((serviceProvider, optionsFactory) =>
            {
                GrpcServicesOptions options = serviceProvider.GetRequiredService<IOptions<GrpcServicesOptions>>().Value;

                ArgumentNullException.ThrowIfNull(options.ProductService);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.ProductService.Address);

                optionsFactory.Address = new Uri(options.ProductService.Address);
            });
    }
}