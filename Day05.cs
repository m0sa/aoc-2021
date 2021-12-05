namespace aoc_2021;

public class Day05
{
    private static readonly Regex _lineMatcher = new Regex(@"(?<x1>\d+),(?<y1>\d+) -> (?<x2>\d+),(?<y2>\d+)");
    public record Line(Vector2 start, Vector2 end);
    public Line ParseLine(string line)
    {
        var match = _lineMatcher.Match(line);
        return new Line(
            new Vector2(int.Parse(match.Groups["x1"].Value), int.Parse(match.Groups["y1"].Value)),
            new Vector2(int.Parse(match.Groups["x2"].Value), int.Parse(match.Groups["y2"].Value))
        );
    }

    public static Vector2 Normalize45(Vector2 vector)
    {
        var length = vector.Length();
        var x = Math.Round(vector.X / length, 4);
        var y = Math.Round(vector.Y / length, 4);
        var n45 = Math.Round(Math.Sqrt(2) / 2, 4);
        if (x == n45) x = 1;
        if (x == -n45) x = -1;
        if (y == n45) y = 1;
        if (y == -n45) y = -1;
        return new Vector2((int)x, (int)y);
    }

    public static IEnumerable<Vector2> Expand(Line line)
    {
        var direction = line.end - line.start;
        var incrementBy = Normalize45(direction);
        var times = (int)Math.Max(Math.Abs(line.start.X - line.end.X), Math.Abs(line.start.Y - line.end.Y));
        for (int i = 0; i <= times; i++)
        {
            yield return line.start + incrementBy * i;
        }
    }

    [Theory]
    [InlineData("Day05_Example01", true, 5)]
    [InlineData("Day05_Input", true, 5280)]
    [InlineData("Day05_Example01", false, 12)]
    [InlineData("Day05_Input", false, 16716)]
    public void Part1(string input, bool onAxisOnly, int expectedResult)
    {
        var lines = LoadData(input).Split(Environment.NewLine).Select(ParseLine);

        if (onAxisOnly)
        {
            lines = lines.Where(l => l.start.Y == l.end.Y || l.start.X == l.end.X);
        }

        var covered = lines
            .SelectMany(Expand)
            .GroupBy(x => x)
            .ToDictionary(x => x, g => g.Count());

        var result = covered.Count(x => x.Value > 1);
        Assert.Equal(expectedResult, result);
    }
}