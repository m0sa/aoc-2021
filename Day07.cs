using Xunit.Abstractions;

namespace aoc_2021;

public class Day07
{
    public static int FuelCost1(int position, int target) => (int)Math.Abs(target - position);
    public static int FuelCost2(int position, int target)
    {
        // sum of arithmetic sequence
        var n = (int)Math.Abs(position - target);
        return n * (1 + n) / 2;
    }

    [Theory]
    [InlineData(nameof(FuelCost1), "Day07_Example01", 37)]
    [InlineData(nameof(FuelCost1), "Day07_Input", 352254)]
    [InlineData(nameof(FuelCost2), "Day07_Example01", 168)]
    [InlineData(nameof(FuelCost2), "Day07_Input", 99053143)]
    public void Tests(string method, string input, int expectedOutput)
    {
        var data = LoadData(input).Split(',').Select(int.Parse);
        var testMethod = typeof(Day07).GetMethod(method, BindingFlags.Public | BindingFlags.Static) ?? throw new ArgumentException($"Method {method} not found", nameof(method));
        var fuelCostMethod = testMethod.CreateDelegate<Func<int, int, int>>();

        var min = data.Min();
        var max = data.Max();

        var minFuel = int.MaxValue;
        var minPos = int.MaxValue;
        for (var i = min; i <= max; i++)
        {
            var fuelCost = data.Sum(x => fuelCostMethod(x, i));
            if (fuelCost < minFuel)
            {
                minFuel = fuelCost;
                minPos = i;
            }
        }

        Assert.Equal(expectedOutput, minFuel);
    }
}