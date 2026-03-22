using Application.Models.Primitives.Enums;
using Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Persistence.Extensions;

public static class NpgsqlDataSourceExtensions
{
    public static void AddNpgsqlDataSource(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(serviceProvider =>
        {
            DatabaseOptions dbOptions =
                serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

            ArgumentException.ThrowIfNullOrWhiteSpace(dbOptions.ConnectionString);

            NpgsqlDataSourceBuilder dataSourceBuilder = new(dbOptions.ConnectionString);

            dataSourceBuilder
                .MapEnum<OrderState>(pgName: "order_state");

            dataSourceBuilder
                .MapEnum<OrderHistoryItemKind>(pgName: "order_history_item_kind");

            return dataSourceBuilder.Build();
        });
    }
}