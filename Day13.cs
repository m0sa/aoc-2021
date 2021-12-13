namespace aoc_2021;
using Map = System.Collections.Immutable.ImmutableHashSet<(int x, int y)>;

public class Day13
{
    private static Map ParseMap(string input) =>
        input
            .Split(Environment.NewLine)
            .TakeWhile(l => l != "")
            .Select(l => l.Split(','))
            .Select(s => (x: int.Parse(s[0]), y: int.Parse(s[1])))
            .ToImmutableHashSet();

    private static Regex INSTRUCTION = new Regex(@"fold along ([xy])=(\d+)", RegexOptions.Compiled);
    private static ImmutableList<Func<Map, Map>> ParseInstrutions(string input) =>
        input
            .Split(Environment.NewLine)
            .Select(l => INSTRUCTION.Match(l))
            .Where(m => m.Success)
            .Select(m => (axis: m.Groups[1].ValueSpan[0], position: int.Parse(m.Groups[2].ValueSpan)))
            .Select(instruction => instruction.axis switch
            {
                'x' => new Func<Map,Map>(map => FoldLeft(map, instruction.position)),
                'y' => map => FoldUp(map, instruction.position),
                _ => throw new InvalidDataException($"unexpected {instruction.axis}"),
            })
            .ToImmutableList();

    private static Map FoldUp(Map map, int at)
        => map
            .Select(p =>
                p.y switch
                {
                    int y when y < at => p,
                    _ => (p.x, 2*at - p.y),
                })
            .ToImmutableHashSet();

    private static Map FoldLeft(Map map, int at)
        => map
            .Select(p =>
                p.x switch
                {
                    int x when x < at => p,
                    _ => (2*at - p.x, p.y),
                })
            .ToImmutableHashSet();

    [Theory]
    [InlineData("Day13_Example01", 17)]
    [InlineData("Day13_Input", 724)]
    public void Test(string input, int part1result)
    {
        // part 1
        var text = LoadData(input);
        var instructions = ParseInstrutions(text);
        var map = ParseMap(text);

        Assert.Equal(part1result, instructions.First()(map).Count);

        // part 2
        var expectedResult = LoadData(input + "_Result");
        map = instructions.Aggregate(map, (m, i) => i(m));
        var maxY = map.Max(p => p.y);
        var maxX = map.Max(p => p.x);
        var result = new System.Text.StringBuilder();
        for (var y = 0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                result.Append(map.Contains((x, y)) ? '#' : '.');
            }
            if (y < maxY)
            {
                result.AppendLine();
            }
        }

        Assert.Equal(expectedResult, result.ToString());
    }
}