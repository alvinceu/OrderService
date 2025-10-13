using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Processing;
using Spectre.Console;

namespace Lab2.Task3;

public sealed class OptionRender : BackgroundService
{
    private TaskCompletionSource<RenderOptions> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public OptionRender(IOptionsMonitor<RenderOptions> monitor)
    {
        monitor.OnChange(option => _tcs.SetResult(option));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            RenderOptions option = await _tcs.Task;
            _tcs = new TaskCompletionSource<RenderOptions>(TaskCreationOptions.RunContinuationsAsynchronously);
            Render(option);
        }
    }

    private void Render(RenderOptions values)
    {
        AnsiConsole.Clear();
        if (!string.IsNullOrWhiteSpace(values.Figlet))
        {
            FigletText figlet = new FigletText(values.Figlet)
                .Centered()
                .Color(Color.Green);
            AnsiConsole.Write(new Panel(figlet).Expand());
        }

        if (!string.IsNullOrWhiteSpace(values.Base64))
        {
            byte[] bytes = Convert.FromBase64String(values.Base64);

            var image = new CanvasImage(bytes);
            image.Mutate(ctx => ctx.Resize(new SixLabors.ImageSharp.Size(24)));

            AnsiConsole.Write(new Panel(Align.Center(image)).Expand());
        }

        if (!string.IsNullOrWhiteSpace(values.Url))
        {
            using var http = new HttpClient();
            byte[] imageBytes = http.GetByteArrayAsync(values.Url).GetAwaiter().GetResult();

            string base64String = Convert.ToBase64String(imageBytes);
            byte[] bytes = Convert.FromBase64String(base64String);

            var image = new CanvasImage(bytes);
            image.Mutate(ctx => ctx.Resize(new SixLabors.ImageSharp.Size(24)));

            AnsiConsole.Write(new Panel(Align.Center(image)).Expand());
        }
    }
}