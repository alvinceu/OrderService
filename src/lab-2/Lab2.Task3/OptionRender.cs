using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Processing;
using Spectre.Console;

namespace Lab2.Task3;

public sealed class OptionRender
{
    private readonly IHttpClientFactory _factory;

    public OptionRender(IHttpClientFactory factory, IOptionsMonitor<RenderOptions> monitor)
    {
        _factory = factory;
        monitor.OnChange(async void (option) => await Render(option));
    }

    private async Task Render(RenderOptions values)
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
            using HttpClient http = _factory.CreateClient();
            byte[] imageBytes = await http.GetByteArrayAsync(values.Url);

            var image = new CanvasImage(imageBytes);
            image.Mutate(ctx => ctx.Resize(new SixLabors.ImageSharp.Size(24)));

            AnsiConsole.Write(new Panel(Align.Center(image)).Expand());
        }
    }
}