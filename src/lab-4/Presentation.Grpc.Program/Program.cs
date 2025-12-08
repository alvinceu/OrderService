using Application.Extensions;
using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Extensions;
using Lab2.Task1.Extensions;
using Lab2.Task2.Extenstions;
using Microsoft.Extensions.Options;
using Presentation.Grpc.Extensions;
using Presentation.Grpc.Program;
using Presentation.Kafka.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddConfigurationServiceRefit();

builder
    .AddConfigurationProvider();

builder
    .Services
    .Configure<DatabaseOptions>(builder.Configuration);

builder
    .Services
    .Configure<ServerOptions>(builder.Configuration.GetSection("GrpcConfigurations"));

builder
    .Services
    .Configure<DatabaseStartUpOptions>(builder.Configuration.GetSection("DatabaseStartUp"));

builder
    .Services
    .AddMigration();

builder
    .Services
    .AddNpgsqlDataSource();

builder
    .Services
    .AddRepositories();

builder
    .Services
    .AddServices();

builder
    .Services
    .AddValidators();

builder
    .Services
    .AddMyGrpc();

builder
    .Services
    .AddHostedService<DatabaseStartUpBackgroundService>();

builder
    .Services
    .AddSerializersAndDeserializers();

builder
    .Services
    .AddKafkaMessageHandler(builder.Configuration);

builder
    .Services
    .AddWrappedDomainOrderService(builder.Configuration);

WebApplication app = builder.Build();

app.UseRouting();

app.MapGrpcEndpoint();

ServerOptions serverOptions =
    app
        .Services
        .GetRequiredService<IOptions<ServerOptions>>().Value;

await app.RunAsync(serverOptions.ServerUrl);