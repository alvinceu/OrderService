namespace Gateway.Configurations;

public sealed record GrpcServicesOptions
{
    public GrpcServiceOptions? OrderService { get; set; }

    public GrpcServiceOptions? ProductService { get; set; }

    public GrpcServiceOptions? OrderProcessingService { get; set; }
}