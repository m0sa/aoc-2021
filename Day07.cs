using Xunit.Abstractions;

namespace aoc_2021;

public class Day07
{
    private readonly ITestOutputHelper _helper;
    public Day07(ITestOutputHelper helper) => _helper = helper;

    [Theory]
    [InlineData("Day07_Example01", 37)]
    [InlineData("Day07_Input", 352254)]
    public void Part01(string input, int expectedOutput)
    {
        var data = LoadData(input).Split(',').Select(int.Parse);

        var min = data.Min();
        var max = data.Max();

        var minFuel = int.MaxValue;
        var minPos = int.MaxValue;
        for (var i = min; i <= max; i++)
        {
            var fuelCost = data.Sum(x => (int)Math.Abs(i - x));
            if (fuelCost < minFuel)
            {
                minFuel = fuelCost;
                minPos = i;
            }
        }

        Assert.Equal(expectedOutput, minFuel);
    }

    [Theory]
    [InlineData("Day07_Example01", 168)]
    [InlineData("Day07_Input", 99053143)]
    public void Part02(string input, int expectedOutput)
    {
        var data = LoadData(input).Split(',').Select(int.Parse);

        var min = data.Min();
        var max = data.Max();

        var minFuel = int.MaxValue;
        var minPos = int.MaxValue;
        for (var i = min; i <= max; i++)
        {
            var fuelCost = data.Sum(x => Enumerable.Range(1, (int)Math.Abs(i - x)).Sum());
            if (fuelCost < minFuel)
            {
                minFuel = fuelCost;
                minPos = i;
            }
        }

        Assert.Equal(expectedOutput, minFuel);
    }
}