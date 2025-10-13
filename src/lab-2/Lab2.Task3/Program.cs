using Lab2.Task1;
using Lab2.Task2;
using Lab2.Task3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder
    .Services
    .AddConfigurationServiceRefit()
    .ConfigureHttpClient(clint => clint.BaseAddress = new Uri("http://localhost:8080"));

builder
    .AddConfigurationProvider(timer);

builder
    .Services
    .AddHostedService<OptionRender>();

builder
    .Services
    .Configure<RenderOptions>(builder.Configuration);

builder
    .Logging
    .ClearProviders();

using IHost host = builder.Build();

IConfigurationUpdaterBackgroundService background = host
    .Services
    .GetRequiredService<IConfigurationUpdaterBackgroundService>();

background.Start();

host.Run();