using Gateway.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderServiceClient = Lab3.Contracts.Services.Order.OrderService.OrderServiceClient;
using ProductServiceClient = Lab3.Contracts.Services.Product.ProductService.ProductServiceClient;

namespace Gateway.Extensions;

public static class GrpcClientExtensions
{
    public static void AddOrderServiceGrpcClient(this IServiceCollection services)
    {
        services
            .AddGrpcClient<OrderServiceClient>((serviceProvider, optionsFactory) =>
            {
                GrpcServicesOptions options = serviceProvider.GetRequiredService<IOptions<GrpcServicesOptions>>().Value;

                ArgumentNullException.ThrowIfNull(options.OrderService);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.OrderService.Address);

                optionsFactory.Address = new Uri(options.OrderService.Address);
            });
    }

    public static void AddProductServiceGrpcClient(this IServiceCollection services)
    {
        services
            .AddGrpcClient<ProductServiceClient>((serviceProvider, optionsFactory) =>
            {
                GrpcServicesOptions options = serviceProvider.GetRequiredService<IOptions<GrpcServicesOptions>>().Value;

                ArgumentNullException.ThrowIfNull(options.ProductService);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.ProductService.Address);

                optionsFactory.Address = new Uri(options.ProductService.Address);
            });
    }

    public static void AddOrderProcessingServiceGrpcClient(this IServiceCollection services)
    {
        services
            .AddGrpcClient<ProductServiceClient>((serviceProvider, optionsFactory) =>
            {
                GrpcServicesOptions options = serviceProvider.GetRequiredService<IOptions<GrpcServicesOptions>>().Value;

                ArgumentNullException.ThrowIfNull(options.OrderProcessingService);
                ArgumentException.ThrowIfNullOrWhiteSpace(options.OrderProcessingService.Address);

                optionsFactory.Address = new Uri(options.OrderProcessingService.Address);
            });
    }
}