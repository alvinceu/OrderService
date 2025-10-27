using Lab2.Task1.Extensions;
using Lab2.Task2.Extenstions;
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
    .AddSingleton<OptionRender>();

builder
    .AddConfigurationProvider();

builder
    .Services
    .Configure<RenderOptions>(builder.Configuration);

builder
    .Logging
    .ClearProviders();

using IHost host = builder.Build();

OptionRender render = host.Services.GetRequiredService<OptionRender>();

host.Run();