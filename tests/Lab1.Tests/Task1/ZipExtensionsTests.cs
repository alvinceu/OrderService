using FluentAssertions;
using Lab1.Task1;
using Xunit;

namespace Lab1.Tests.Task1;

public sealed class ZipExtensionsTests
{
    public static object[][] GetScenario2Data()
    {
        return new object[][]
        {
            [
                new[] { 1, 2, 3 },
                new int[][]
                {
                    [4, 5, 6],
                    [7, 8, 9],
                    [10, 11, 12],
                },
                new int[][]
                {
                    [1, 4, 7, 10],
                    [2, 5, 8, 11],
                    [3, 6, 9, 12],
                }

            ],
            [
                new[] { "1", "2", "3" },
                new string[][]
                {
                    ["4", "5", "6"],
                    ["7", "8", "9"],
                    ["10", "11", "12"],
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

    public static object[][] GetScenario3Data()
    {
        return new object[][]
        {
            [
                new[] { 1, 2 },
                new int[][]
                {
                    [4, 5, 6],
                    [7, 8, 9],
                    [10, 11, 12],
                },
                new int[][]
                {
                    [1, 4, 7, 10],
                    [2, 5, 8, 11],
                }

            ],
            [
                new[] { "1", "2" },
                new string[][]
                {
                    ["4", "5", "6"],
                    ["7", "8", "9"],
                    ["10", "11", "12"],
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
    public void ZipAll_Scenario1()
    {
        var collection = new List<int> { 1, 2, 3 };
        IEnumerable<int[]> zipped = collection.ZipAll();

        int[][] result = zipped.ToArray();

        result.Should().BeEquivalentTo(new int[][] { [1], [2], [3] });
    }

    [Theory]
    [MemberData(nameof(GetScenario2Data))]
    public void ZipAll_Scenario2<T>(IEnumerable<T> seed, IEnumerable<T>[] collections, T[][] expected)
    {
        seed.ZipAll(collections).Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(GetScenario3Data))]
    public void ZipAll_Scenario3<T>(IEnumerable<T> seed, IEnumerable<T>[] collections, T[][] expected)
    {
        seed.ZipAll(collections).Should().BeEquivalentTo(expected);
    }
}