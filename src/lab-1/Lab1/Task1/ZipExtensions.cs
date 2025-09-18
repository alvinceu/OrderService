namespace Lab1.Task1;

public static class ZipExtensions
{
    public static IEnumerable<TResult> ZipAll<TCollection, TResult>(
        this IEnumerable<TCollection> seed,
        Func<TCollection[], TResult> resultSelector,
        params IEnumerable<TCollection>[] collections)
    {
        ArgumentNullException.ThrowIfNull(seed);

        ArgumentNullException.ThrowIfNull(resultSelector);

        ArgumentNullException.ThrowIfNull(collections);

        return ZipIterator(seed, resultSelector, collections);
    }

    public static IEnumerable<TCollection[]> ZipAll<TCollection>(
        this IEnumerable<TCollection> seed,
        params IEnumerable<TCollection>[] collections)
    {
        ArgumentNullException.ThrowIfNull(seed);

        ArgumentNullException.ThrowIfNull(collections);

        return ZipIterator(seed, x => x,  collections);
    }

    private static IEnumerable<TResult> ZipIterator<TCollection, TResult>(
        IEnumerable<TCollection> seed,
        Func<TCollection[], TResult> resultSelector,
        params IEnumerable<TCollection>[] collections)
    {
        var enumerators = new IEnumerator<TCollection>[collections.Length + 1];

        enumerators[0] = seed.GetEnumerator();

        for (int i = 0; i < collections.Length; i++)
        {
            enumerators[i + 1] = collections[i].GetEnumerator();
        }

        try
        {
            while (enumerators.All(e => e.MoveNext()))
            {
                yield return resultSelector(enumerators.Select(e => e.Current).ToArray());
            }
        }
        finally
        {
            foreach (IEnumerator<TCollection> enumerator in enumerators)
            {
                enumerator.Dispose();
            }
        }
    }
}