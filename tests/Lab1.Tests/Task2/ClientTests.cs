using FluentAssertions;
using Lab1.Task2;
using NSubstitute;
using Xunit;

namespace Lab1.Tests.Task2;

public class ClientTests
{
    private readonly ILibraryOperationService _mockLibraryService;

    private readonly Client _client;

    public ClientTests()
    {
        _mockLibraryService = Substitute.For<ILibraryOperationService>();
        _client = new Client(_mockLibraryService);
    }

    [Fact]
    public async Task ClientScenario1()
    {
        _mockLibraryService
            .When(x => x.BeginOperation(
                Arg.Any<Guid>(),
                Arg.Any<RequestModel>(),
                Arg.Any<CancellationToken>()))
            .Do(x => Task.Run(async () =>
            {
                await Task.Delay(100).ConfigureAwait(false);
                _client.HandleOperationResult(x.ArgAt<Guid>(0), x.ArgAt<RequestModel>(1).Data);
            }));
        var requestModel = new RequestModel("test", "data"u8.ToArray());
        CancellationTokenSource cts = new();

        ResponseModel response = await _client.SendAsync(requestModel, cts.Token);

        response.Should().BeEquivalentTo(new ResponseModel("data"u8.ToArray()));
    }

    [Fact]
    public async Task ClientScenario2()
    {
        _mockLibraryService
            .When(x => x.BeginOperation(
                Arg.Any<Guid>(),
                Arg.Any<RequestModel>(),
                Arg.Any<CancellationToken>()))
            .Do(x => Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    _client.HandleOperationError(x.ArgAt<Guid>(0), new Exception());
                }));
        var requestModel = new RequestModel("test", "data"u8.ToArray());
        CancellationTokenSource cts = new();

        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await _client.SendAsync(requestModel, cts.Token));
    }

    [Fact]
    public async Task ClientScenario3()
    {
        var requestModel = new RequestModel("test", "data"u8.ToArray());
        CancellationTokenSource cts = new();

        await cts.CancelAsync();

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _client.SendAsync(requestModel, cts.Token));
    }

    [Fact]
    public async Task ClientScenario4()
    {
        _mockLibraryService
            .When(x => x.BeginOperation(
                Arg.Any<Guid>(),
                Arg.Any<RequestModel>(),
                Arg.Any<CancellationToken>()))
            .Do(x => Task.Run(async () =>
            {
                await Task.Delay(200).ConfigureAwait(false);
                CancellationToken ct = x.ArgAt<CancellationToken>(2);
                try
                {
                    ct.ThrowIfCancellationRequested();
                    _client.HandleOperationResult(x.ArgAt<Guid>(0), x.ArgAt<RequestModel>(1).Data);
                }
                catch (Exception e)
                {
                    _client.HandleOperationError(x.ArgAt<Guid>(0), e);
                }
            }));
        var requestModel = new RequestModel("test", "data"u8.ToArray());
        CancellationTokenSource cts = new();

        cts.CancelAfter(100);

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _client.SendAsync(requestModel, cts.Token));
    }

    [Fact]
    public async Task ClientScenario5()
    {
        _mockLibraryService
            .When(x => x.BeginOperation(
                Arg.Any<Guid>(),
                Arg.Any<RequestModel>(),
                Arg.Any<CancellationToken>()))
            .Do(x =>
            {
                _client.HandleOperationResult(x.ArgAt<Guid>(0), x.ArgAt<RequestModel>(1).Data);
            });
        var requestModel = new RequestModel("test", "data"u8.ToArray());
        CancellationTokenSource cts = new();

        ResponseModel response = await _client.SendAsync(requestModel, cts.Token);

        response.Should().BeEquivalentTo(new ResponseModel("data"u8.ToArray()));
    }

    [Fact]
    public async Task ClientScenario6()
    {
        _mockLibraryService
            .When(x => x.BeginOperation(
                Arg.Any<Guid>(),
                Arg.Any<RequestModel>(),
                Arg.Any<CancellationToken>()))
            .Do(x =>
            {
                _client.HandleOperationError(x.ArgAt<Guid>(0), new Exception());
            });
        var requestModel = new RequestModel("test", "data"u8.ToArray());
        CancellationTokenSource cts = new();

        await Assert.ThrowsAsync<Exception>(async () =>
            await _client.SendAsync(requestModel, cts.Token));
    }

    [Fact]
    public async Task ClientScenario7()
    {
        CancellationTokenSource cts = new();
        _mockLibraryService
            .When(x => x.BeginOperation(
                Arg.Any<Guid>(),
                Arg.Any<RequestModel>(),
                Arg.Any<CancellationToken>()))
            .Do(x =>
            {
                CancellationToken ct = cts.Token;
                cts.Cancel();
                try
                {
                    ct.ThrowIfCancellationRequested();
                    _client.HandleOperationResult(x.ArgAt<Guid>(0), x.ArgAt<RequestModel>(1).Data);
                }
                catch (Exception exception)
                {
                    _client.HandleOperationError(x.ArgAt<Guid>(0), exception);
                }
            });
        var requestModel = new RequestModel("test", "data"u8.ToArray());

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
            await _client.SendAsync(requestModel, cts.Token));
    }
}