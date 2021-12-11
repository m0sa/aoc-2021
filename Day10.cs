namespace aoc_2021;

public class Day10
{
    private static readonly ImmutableDictionary<char, char> PAIRS = new Dictionary<char, char>()
    {
        {'(', ')'},
        {'[', ']'},
        {'{', '}'},
        {'<', '>'},
    }.ToImmutableDictionary();

    private enum LineState
    {
        Valid,
        Corrupted,
        Incomplete,
    }

    private static (LineState state, char invalid, IEnumerable<char> expected) ValidateLine(IEnumerable<char> line)
    {
        var stack = new Stack<char>();
        foreach (var ch in line)
        {
            if (PAIRS.TryGetValue(ch, out var close))
            {
                stack.Push(close);
            }
            else if (stack.Pop() is char expected && expected != ch)
            {
                return (LineState.Corrupted, ch, stack);
            }
        }

        return (stack.Count == 0 ? LineState.Valid : LineState.Incomplete, default, stack);
    }

    [Theory]
    [InlineData("Day10_Example", 26397)]
    [InlineData("Day10_Input", 294195)]
    public void Part1(string input, int expectedResult)
    {
        var lines = LoadData(input).Split(Environment.NewLine);

        var result = lines.Select(ValidateLine).Where(l => l.state == LineState.Corrupted).Sum(l => l.invalid switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => throw new InvalidDataException(),
        });

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("Day10_Example", 288957)]
    [InlineData("Day10_Input", 3490802734)]
    public void Part2(string input, long expectedResult)
    {
        var lines = LoadData(input).Split(Environment.NewLine);
        var scores = lines
            .Select(ValidateLine)
            .Where(l => l.state == LineState.Incomplete)
            .Select(l => l.expected.Aggregate(0L, (total, next) => total * 5 + next switch {
                ')' => 1,
                ']' => 2,
                '}' => 3,
                '>' => 4,
                _ => throw new InvalidDataException(),
            }))
            .OrderBy(x => x)
            .ToArray();
        var result = scores[scores.Length / 2];

        Assert.Equal(expectedResult, result);
    }
}