using Integration.Kafka.Commons;
using Integration.Kafka.Configurations;
using Integration.Kafka.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Kafka.Extensions;

public static class KafkaMessageConsumerExtensions
{
    public static void AddBatchingKafkaHandler<TKey, TValue, THandler>(this IServiceCollection servicesCollection)
        where THandler : class, IKafkaMessageHandler<TKey, TValue>
    {
        servicesCollection
            .AddScoped<IKafkaMessageHandler<TKey, TValue>, THandler>();

        servicesCollection
            .AddScoped<KafkaMessageIngestWorker<TKey, TValue>>();

        servicesCollection
            .AddScoped<BatchingKafkaMessageHandler<TKey, TValue>>();

        servicesCollection
            .AddHostedService<KafkaMessageDispatcher<TKey, TValue>>();
    }

    public static void AddKafkaConsumerOptions(this IServiceCollection servicesCollection, IConfiguration configuration)
    {
        servicesCollection
            .AddOptions<BatchingKafkaMessageHandlerOptions>().Bind(configuration.GetSection("Kafka:Batching"));

        servicesCollection
            .AddOptions<KafkaBatchingBoundedChannelOptions>().Bind(configuration.GetSection("Kafka:Batching:Channel"));

        servicesCollection
            .AddOptions<KafkaMessageConsumerConfigOptions>().Bind(configuration.GetSection("Kafka:Consumer"));
    }
}