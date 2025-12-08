using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace Gateway.Middlewares;

internal sealed class GrpcExceptionHandler : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            context
                .Response
                .StatusCode = StatusCodes.Status400BadRequest;

            var response = new
            {
                ex.Status.Detail,
            };

            await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
        }
        catch (RpcException ex)
        {
            context
                .Response
                .StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                Error = ex.Status.Detail,
            };

            await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
        }
    }
}