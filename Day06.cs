using Xunit.Abstractions;

namespace aoc_2021;

public class Day06
{

    public ImmutableList<int> Day(IEnumerable<int> generation, int halflife = 7)
    {
        return generation
            .Select(x => { var next = x - 1;  return next < 0 ? halflife - 1 : next; })
            .Concat(Enumerable.Repeat(halflife + 1, generation.Where(x => x == 0).Count()))
            .ToImmutableList();
    }
    private readonly ITestOutputHelper _output;
    public Day06(ITestOutputHelper output) => _output = output;
    [Theory]
    [InlineData("Day06_Example01", 1, 5)]
    [InlineData("Day06_Example01", 2, 6)]
    [InlineData("Day06_Example01", 3, 7)]
    [InlineData("Day06_Example01", 4, 9)]
    [InlineData("Day06_Example01", 18, 26)]
    [InlineData("Day06_Example01", 80, 5934)]
    [InlineData("Day06_Input", 80, 343441)]
    public void Part1(string intput, int days, int expectedCount)
    {
        var generation = LoadData(intput).Split(',').Select(int.Parse).ToImmutableList();
        for (var i = 0; i < days; i++)
        {
            generation = Day(generation);
        }
        Assert.Equal(expectedCount, generation.Count);
    }
}