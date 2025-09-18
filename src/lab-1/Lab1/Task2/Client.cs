using System.Collections.Concurrent;
using System.Diagnostics;

namespace Lab1.Task2;

public sealed class Client(ILibraryOperationService libraryService) : IRequestClient, ILibraryOperationHandler
{
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<ResponseModel>> _tcsMap = new();

    public Task<ResponseModel> SendAsync(RequestModel request, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<ResponseModel>(cancellationToken,  TaskCreationOptions.RunContinuationsAsynchronously);

        if (cancellationToken.IsCancellationRequested)
        {
            tcs.SetCanceled(cancellationToken);
            return tcs.Task;
        }

        var requestId = Guid.NewGuid();
        _tcsMap[requestId] = tcs;

        libraryService.BeginOperation(requestId, request, cancellationToken);

        return tcs.Task;
    }

    public void HandleOperationResult(Guid requestId, byte[] data)
    {
        if (!_tcsMap.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? tcs))
            throw new UnreachableException();

        tcs.SetResult(new ResponseModel(data));
    }

    public void HandleOperationError(Guid requestId, Exception exception)
    {
        if (!_tcsMap.TryRemove(requestId, out TaskCompletionSource<ResponseModel>? tcs))
            throw new UnreachableException();

        if (exception is OperationCanceledException)
            tcs.SetCanceled();
        else
            tcs.SetException(exception);
    }
}