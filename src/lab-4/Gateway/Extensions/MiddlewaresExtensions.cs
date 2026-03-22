using Gateway.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Extensions;

public static class MiddlewaresExtensions
{
    public static void AddMyExceptionHandler(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<GrpcExceptionHandler>();
    }

    public static void UseMyExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<GrpcExceptionHandler>();
    }
}