namespace aoc_2021;

public class Day01
{
    public static int SonarSweep_Part01(IEnumerable<int> depths) =>
        depths
            .Aggregate(
                (increases: 0, previous: (int?)null),
                (state, cur) => state.previous == null
                    ? (increases:0, previous: cur)
                    : (increases: state.increases + (state.previous < cur ? 1 : 0), previous: cur),
                state => state.increases);

    [Theory]
    [InlineData("Day01_Example01", 7)]
    [InlineData("Day01_Input01", 1602)]
    public void Day01_Part01(string resource, int expectedResult)
    {
        var data = LoadData(resource).Split(Environment.NewLine).Select(int.Parse);
        Assert.Equal(expectedResult, SonarSweep_Part01(data));
    }

    public static int SonarSweep_Part02(IEnumerable<int> depths) =>
        depths
            .Aggregate(
                (increases: 0, previous: new int[0].ToImmutableArray(), previousSum: (int?) null),
                (state, cur) => {
                    var current = state.previous.Append(cur).TakeLast(3).ToImmutableArray();
                    var sum = current.Sum();
                    var increases = state.previousSum != null && sum > state.previousSum
                        ? state.increases + 1
                        : state.increases;
                    return (increases, previous: current, previousSum: current.Length == 3 ? sum : null);
                },
                state => state.increases);

    [Theory]
    [InlineData("Day01_Example01", 5)]
    [InlineData("Day01_Input01", 1633)]
    public void Day01_Part02(string resource, int expectedResult)
    {
        var data = LoadData(resource).Split(Environment.NewLine).Select(int.Parse);
        Assert.Equal(expectedResult, SonarSweep_Part02(data));
    }
}