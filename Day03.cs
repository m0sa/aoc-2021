namespace aoc_2021;

public class Day03
{

    [Theory]
    [InlineData("Day03_Example01", 198)]
    [InlineData("Day03_Input", 741950)]
    public void Day03_Part01(string resource, int expectedResult)
    {
        var data = LoadData(resource)
            .Split(Environment.NewLine)
            .Select(x => x.ToCharArray().Select(c => c == '0').ToImmutableArray())
            .ToImmutableArray();
        var gamma = 0;
        var epsilon = 0;
        var len = data[0].Length;

        for (int i = 0, exp = 1; i < len; i++, exp *= 2)
        {
            var bitPos = len - i - 1;
            var ones = data.Select(x => x[bitPos]).Count(b => b);
            var zeroes = data.Length - ones;
            epsilon += ones > zeroes ? exp : 0;
            gamma += zeroes > ones ? exp : 0;
        }

        Assert.Equal(gamma * epsilon, expectedResult);
    }
}