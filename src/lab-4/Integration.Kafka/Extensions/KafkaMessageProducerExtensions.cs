using Integration.Kafka.Commons;
using Integration.Kafka.Configurations;
using Integration.Kafka.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Kafka.Extensions;

public static class KafkaMessageProducerExtensions
{
    public static void AddKafkaProducer<TKey, TValue>(this IServiceCollection servicesCollection)
    {
        servicesCollection
            .AddSingleton<IKafkaMessageProducer<TKey, TValue>, KafkaMessageProducer<TKey, TValue>>();
    }

    public static void AddKafkaProducerOptions(this IServiceCollection servicesCollection, IConfiguration configuration)
    {
        servicesCollection
            .AddOptions<KafkaProducerOptions>().Bind(configuration.GetSection("Kafka:Producer"));
    }
}