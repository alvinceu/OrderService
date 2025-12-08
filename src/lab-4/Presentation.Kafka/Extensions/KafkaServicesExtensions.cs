using Application.Contracts.Services;
using Confluent.Kafka;
using Integration.Kafka.Commons;
using Integration.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Kafka.Contracts;
using Presentation.Kafka.Services;

namespace Presentation.Kafka.Extensions;

public static class KafkaServicesExtensions
{
    public static void AddWrappedDomainOrderService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddKafkaProducerOptions(configuration);

        serviceCollection
            .AddKafkaProducer<OrderCreationKey, OrderCreationValue>();

        ServiceDescriptor descriptor =
            serviceCollection
                .First(serviceDescriptor => serviceDescriptor.ServiceType == typeof(IOrderService));

        ArgumentNullException.ThrowIfNull(descriptor.ImplementationType);

        Func<IServiceProvider, IOrderService> factory =
            serviceProvider => (IOrderService)ActivatorUtilities.CreateInstance(serviceProvider, descriptor.ImplementationType);

        serviceCollection.Remove(descriptor);

        serviceCollection.Add(ServiceDescriptor.Describe(
            descriptor.ServiceType,
            serviceProvider =>
            {
                IKafkaMessageProducer<OrderCreationKey, OrderCreationValue> producer =
                    serviceProvider
                        .GetRequiredService<IKafkaMessageProducer<OrderCreationKey, OrderCreationValue>>();

                return new WrappedOrderService(factory(serviceProvider), producer);
            },
            descriptor.Lifetime));
    }

    public static void AddKafkaMessageHandler(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddKafkaConsumerOptions(configuration);

        serviceCollection
            .AddBatchingKafkaHandler<OrderProcessingKey, OrderProcessingValue, KafkaMessageHandler>();
    }

    public static void AddSerializersAndDeserializers(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<ISerializer<OrderCreationKey>, ProtoSerializer<OrderCreationKey>>();

        serviceCollection
            .AddSingleton<ISerializer<OrderCreationValue>, ProtoSerializer<OrderCreationValue>>();

        serviceCollection
            .AddSingleton<IDeserializer<OrderProcessingKey>, ProtoDeserializer<OrderProcessingKey>>();

        serviceCollection
            .AddSingleton<IDeserializer<OrderProcessingValue>, ProtoDeserializer<OrderProcessingValue>>();
    }
}