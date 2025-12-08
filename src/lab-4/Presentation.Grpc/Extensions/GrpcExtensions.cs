using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Grpc.Interceptors;
using Presentation.Grpc.Services;

namespace Presentation.Grpc.Extensions;

public static class GrpcExtensions
{
    public static void MapGrpcEndpoint(this IApplicationBuilder builder)
    {
        builder.UseEndpoints(endpoints =>
        {
            endpoints
                .MapGrpcService<OrderServiceGrpc>();

            endpoints
                .MapGrpcService<ProductServiceGrpc>();
        });
    }

    public static void AddMyGrpc(this IServiceCollection services)
    {
        services.AddGrpc(options =>
            options.Interceptors.Add<ExceptionInterceptor>());
    }
}