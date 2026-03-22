using Application.Abstractions.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Extensions;

public static class RepositoriesExtensions
{
    public static void AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IProductRepository, ProductRepository>();

        serviceCollection
            .AddSingleton<IOrderRepository, OrderRepository>();

        serviceCollection
            .AddSingleton<IOrderItemRepository, OrderItemRepository>();

        serviceCollection
            .AddSingleton<IOrderHistoryItemRepository, OrderHistoryItemRepository>();
    }
}