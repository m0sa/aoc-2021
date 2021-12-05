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

    public static IEnumerable<Vector2> Expand(Line line, Func<Vector2, int> getCoordinate, Vector2 incrementBy)
    {
        var (start, end) = getCoordinate(line.start) < getCoordinate(line.end) ? (line.start, line.end) : (line.end, line.start);

        var from = getCoordinate(start);
        var to = getCoordinate(end);
        var count = to - from;
        for (int i = 0; i <= count; i++)
        {
            yield return start + incrementBy * i;
        }
    }
    public static bool IsHorizontal(Line line) => line.start.Y == line.end.Y;
    public static bool Isvertical(Line line) => line.start.X == line.end.X;

    [Theory]
    [InlineData("Day05_Example01", 5)]
    [InlineData("Day05_Input", 5280)]
    public void Part1(string input, int expectedResult)
    {
        var lines = LoadData(input).Split(Environment.NewLine).Select(ParseLine);

        var covered = lines
            .SelectMany(line
                => IsHorizontal(line) ? Expand(line, v => (int)v.X, Vector2.UnitX)
                : Isvertical(line) ? Expand(line, v => (int)v.Y, Vector2.UnitY)
                : new Vector2[0])
            .GroupBy(x => x)
            .ToDictionary(x => x, g => g.Count());

        var result = covered.Count(x => x.Value > 1);
        Assert.Equal(expectedResult, result);
    }
}