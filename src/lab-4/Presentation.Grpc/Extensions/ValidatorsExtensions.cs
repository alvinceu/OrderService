using Lab3.Contracts.Services.Order;
using Lab3.Contracts.Services.Product;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Validators;

namespace Presentation.Grpc.Extensions;

public static class ValidatorsExtensions
{
    public static void AddValidators(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IRequestValidator<AddOrderItemRequest>, AddOrderItemRequestValidator>();

        serviceCollection
            .AddScoped<IRequestValidator<CreateOrderRequest>, CreateOrderRequestValidator>();

        serviceCollection
            .AddScoped<IRequestValidator<FindOrderHistoryItemsRequest>, FindOrderHistoryItemsRequestValidator>();

        serviceCollection
            .AddScoped<IRequestValidator<RemoveOrderItemRequest>, RemoveOrderItemRequestValidator>();

        serviceCollection
            .AddScoped<IRequestValidator<SetOrderStateProcessingRequest>, SetOrderStateProcessingRequestValidator>();

        serviceCollection
            .AddScoped<IRequestValidator<SetOrderStateCancelledRequest>, SetOrderStateCancelledRequestValidator>();

        serviceCollection
            .AddScoped<IRequestValidator<CreateProductRequest>, CreateProductRequestValidator>();
    }
}