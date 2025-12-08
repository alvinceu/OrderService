using Gateway.Configurations;
using Gateway.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .Configure<GrpcServicesOptions>(builder.Configuration.GetSection("GrpcServices"));

builder
    .Services
    .AddServiceClients();

builder
    .Services
    .AddMyGrpcClient();

builder
    .Services
    .AddMyExceptionHandler();

builder
    .Services
    .AddControllers();

builder
    .Services
    .ConfigureHttpJsonOptions(options =>
    {
        options
            .SerializerOptions
            .Converters
            .Add(new System.Text.Json.Serialization.JsonStringEnumConverter(
                System.Text.Json.JsonNamingPolicy.CamelCase));
    });

builder
    .Services
    .AddSwaggerGen(genOptions =>
    {
        genOptions.SupportNonNullableReferenceTypes();
        genOptions.UseAllOfForInheritance();
        genOptions.UseInlineDefinitionsForEnums();
    });

WebApplication app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseMyExceptionHandler();

app.MapControllers();

await app.RunAsync();