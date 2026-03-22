using Application.Contracts.Services;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServicesExtensions
{
    public static void AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IProductService, ProductService>();

        serviceCollection
            .AddSingleton<IOrderService, OrderService>();
    }
}