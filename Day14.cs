namespace aoc_2021;
using System.Text;

public class Day14
{

    private static void Increment(IDictionary<string, long> store, string pair, long by = 1L) =>
        store[pair] = store.TryGetValue(pair, out var value) ? value + by : by;

    [Theory]
    [InlineData("Day14_Example01", 10, 1588)]
    [InlineData("Day14_Input", 10, 2975)]
    [InlineData("Day14_Example01", 1, 1)]
    [InlineData("Day14_Example01", 2, 5)]
    [InlineData("Day14_Example01", 3, 7)]
    [InlineData("Day14_Example01", 40, 2188189693529)]
    [InlineData("Day14_Input", 40, 3015383850689)]
    public void Test(string input, int steps, long expected)
    {
        var lines = LoadData(input).Split(Environment.NewLine);
        var rules = lines.Skip(2).Select(l => l.Split(" -> ")).ToImmutableDictionary(s => s[0], s => s[1][0]);

        var current = new Dictionary<string, long>();
        var firstLine = lines[0];
        for (var i = 1; i < firstLine.Length; i++)
        {
            Increment(current, firstLine.Substring(i - 1, 2));
        }

        for (var s = 0; s < steps; s++)
        {
            var next = new Dictionary<string, long>();
            foreach(var (sequence, count) in current)
            {
                if (rules.TryGetValue(sequence, out var insert))
                {
                    Increment(next, $"{sequence[0]}{insert}", count);
                    Increment(next, $"{insert}{sequence[1]}", count);
                }
                else
                {
                    Increment(next, sequence, count);
                }
            }
            current = next;
        }

        var counts = new Dictionary<string, long>();
        foreach(var (sequence, count) in current)
        {
            Increment(counts, sequence.Substring(0, 1), count);
            Increment(counts, sequence.Substring(1, 1), count);
        }
        // every character is counted twice, except the first and last one
        Increment(counts, firstLine.Substring(0, 1));
        Increment(counts, firstLine.Substring(firstLine.Length - 1, 1));

        var result = counts.MaxBy(x => x.Value).Value / 2 - counts.MinBy(x => x.Value).Value / 2;
        Assert.Equal(expected, result);
    }
}