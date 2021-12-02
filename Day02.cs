namespace aoc_2021;

public class Day02
{
    public static Vector2 ParseLine(string line)
    {
        var split = line.Split(Space);
        var units = int.Parse(split[1]);
        return split[0] switch
        {
            "forward" => Vector2.UnitX * units,
            "up" => Vector2.UnitY * -1 * units,
            "down" => Vector2.UnitY * units
        };
    }

    public static int Dive_Part01(IEnumerable<string> commands) =>
        commands.Select(ParseLine).Aggregate(Vector2.Zero, (sum, cur) => sum + cur, pos => (int)Math.Round(pos.X * pos.Y));

    [Theory]
    [InlineData("Day02_Example01", 150)]
    [InlineData("Day02_Input", 2322630)]
    public void Day02_Part01(string resource, int expectedResult)
    {
        var data = LoadData(resource).Split(Environment.NewLine);
        Assert.Equal(expectedResult, Dive_Part01(data));
    }
}