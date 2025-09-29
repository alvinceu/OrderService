using FluentAssertions;
using Lab1.Task1;
using Xunit;

namespace Lab1.Tests.Task1;

public sealed class ZipExtensionsAsyncTests
{
    private static readonly int[] Scenario2DataSourceArray0 = [1, 2, 3];
    private static readonly int[] Scenario2DataSourceArray1 = [4, 5, 6];
    private static readonly int[] Scenario2DataSourceArray2 = [7, 8, 9];
    private static readonly int[] Scenario2DataSourceArray3 = [10, 11, 12];

    private static readonly string[] Scenario2DataSourceArray4 = ["1", "2", "3"];
    private static readonly string[] Scenario2DataSourceArray5 = ["4", "5", "6"];
    private static readonly string[] Scenario2DataSourceArray6 = ["7", "8", "9"];
    private static readonly string[] Scenario2DataSourceArray7 = ["10", "11", "12"];

    public static object[][] GetScenario2Data()
    {
        return new object[][]
        {
            [
                Scenario2DataSourceArray0.ToAsyncEnumerable(),
                new[]
                {
                    Scenario2DataSourceArray1.ToAsyncEnumerable(),
                    Scenario2DataSourceArray2.ToAsyncEnumerable(),
                    Scenario2DataSourceArray3.ToAsyncEnumerable(),
                },
                new int[][]
                {
                    [1, 4, 7, 10],
                    [2, 5, 8, 11],
                    [3, 6, 9, 12],
                }

            ],
            [
                Scenario2DataSourceArray4.ToAsyncEnumerable(),
                new[]
                {
                    Scenario2DataSourceArray5.ToAsyncEnumerable(),
                    Scenario2DataSourceArray6.ToAsyncEnumerable(),
                    Scenario2DataSourceArray7.ToAsyncEnumerable(),
                },
                new string[][]
                {
                    ["1", "4", "7", "10"],
                    ["2", "5", "8", "11"],
                    ["3", "6", "9", "12"],
                }

            ],
        };
    }

    private static readonly int[] Scenario3DataSourceArray0 = [1, 2];
    private static readonly int[] Scenario3DataSourceArray1 = [4, 5, 6];
    private static readonly int[] Scenario3DataSourceArray2 = [7, 8, 9];
    private static readonly int[] Scenario3DataSourceArray3 = [10, 11, 12];

    private static readonly string[] Scenario3DataSourceArray4 = ["1", "2"];
    private static readonly string[] Scenario3DataSourceArray5 = ["4", "5", "6"];
    private static readonly string[] Scenario3DataSourceArray6 = ["7", "8", "9"];
    private static readonly string[] Scenario3DataSourceArray7 = ["10", "11", "12"];

    public static object[][] GetScenario3Data()
    {
        return new object[][]
        {
            [
                Scenario3DataSourceArray0.ToAsyncEnumerable(),
                new[]
                {
                    Scenario3DataSourceArray1.ToAsyncEnumerable(),
                    Scenario3DataSourceArray2.ToAsyncEnumerable(),
                    Scenario3DataSourceArray3.ToAsyncEnumerable(),
                },
                new int[][]
                {
                    [1, 4, 7, 10],
                    [2, 5, 8, 11],
                }

            ],
            [
                Scenario3DataSourceArray4.ToAsyncEnumerable(),
                new[]
                {
                    Scenario3DataSourceArray5.ToAsyncEnumerable(),
                    Scenario3DataSourceArray6.ToAsyncEnumerable(),
                    Scenario3DataSourceArray7.ToAsyncEnumerable(),
                },
                new string[][]
                {
                    ["1", "4", "7", "10"],
                    ["2", "5", "8", "11"],
                }

            ],
        };
    }

    [Fact]
    public async Task ZipAllAsync_Scenario1()
    {
        IAsyncEnumerable<int> collection = new List<int> { 1, 2, 3 }.ToAsyncEnumerable();
        IAsyncEnumerable<int[]> zipped = collection.ZipAllAsync();

        int[][] result = await zipped.ToArrayAsync();

        result.Should().BeEquivalentTo(new int[][] { [1], [2], [3] });
    }

    [Theory]
    [MemberData(nameof(GetScenario2Data))]
    public async Task ZipAllAsync_Scenario2<T>(IAsyncEnumerable<T> seed, IAsyncEnumerable<T>[] collections, T[][] expected)
    {
        (await seed.ZipAllAsync(collections: collections).ToArrayAsync()).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(GetScenario3Data))]
    public async Task ZipAllAsync_Scenario3<T>(IAsyncEnumerable<T> seed, IAsyncEnumerable<T>[] collections, T[][] expected)
    {
        (await seed.ZipAllAsync(collections: collections).ToArrayAsync()).Should().BeEquivalentTo(expected);
    }
}