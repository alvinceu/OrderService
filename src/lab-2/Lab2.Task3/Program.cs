using Lab2.Task1;
using Lab2.Task2;
using Lab2.Task3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder
    .Services
    .AddConfigurationServiceRefit();

builder
    .Services
    .AddHostedService<OptionRender>();

builder
    .AddConfigurationProvider();

builder
    .Services
    .Configure<RenderOptions>(builder.Configuration);

builder
    .Logging
    .ClearProviders();

using IHost host = builder.Build();

host.Run();