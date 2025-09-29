using System.Runtime.CompilerServices;

namespace Lab1.Task1;

public static class ZipExtensionsAsync
{
    public static IAsyncEnumerable<TResult> ZipAllAsync<TCollection, TResult>(
        this IAsyncEnumerable<TCollection> seed,
        Func<TCollection[], TResult> resultSelector,
        CancellationToken cancellationToken = default,
        params IAsyncEnumerable<TCollection>[] collections)
    {
        ArgumentNullException.ThrowIfNull(seed);

        ArgumentNullException.ThrowIfNull(resultSelector);

        ArgumentNullException.ThrowIfNull(collections);

        return ZipIteratorAsync(seed, resultSelector, cancellationToken, collections);
    }

    public static IAsyncEnumerable<TCollection[]> ZipAllAsync<TCollection>(
        this IAsyncEnumerable<TCollection> seed,
        CancellationToken cancellationToken = default,
        params IAsyncEnumerable<TCollection>[] collections)
    {
        ArgumentNullException.ThrowIfNull(seed);

        ArgumentNullException.ThrowIfNull(collections);

        return ZipIteratorAsync(seed, x => x, cancellationToken, collections);
    }

    private static async IAsyncEnumerable<TResult> ZipIteratorAsync<TResult, TCollection>(
        IAsyncEnumerable<TCollection> seed,
        Func<TCollection[], TResult> resultSelector,
        [EnumeratorCancellation] CancellationToken cancellationToken = default,
        params IAsyncEnumerable<TCollection>[] collections)
    {
        var enumerators = new IAsyncEnumerator<TCollection>[collections.Length + 1];

        enumerators[0] = seed.GetAsyncEnumerator(cancellationToken);

        for (int i = 0; i < collections.Length; i++)
        {
            enumerators[i + 1] = collections[i].GetAsyncEnumerator(cancellationToken);
        }

        try
        {
            while (true)
            {
                IEnumerable<Task<bool>> tasks = enumerators.Select(e => e.MoveNextAsync().AsTask());
                bool[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

                if (results.Any(r => !r))
                    yield break;
                yield return resultSelector(enumerators.Select(e => e.Current).ToArray());
            }
        }
        finally
        {
            foreach (IAsyncEnumerator<TCollection> enumerator in enumerators)
            {
                await enumerator.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}