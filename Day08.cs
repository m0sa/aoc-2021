namespace aoc_2021;

public class Day08
{
    [Flags]
    private enum Segments : byte
    {
        a = 0b0000001,
        b = 0b0000010,
        c = 0b0000100,
        d = 0b0001000,
        e = 0b0010000,
        f = 0b0100000,
        g = 0b1000000,
    }
    private static Segments ParseSegments(string value) =>
            value
            .Select((_, i) => Enum.Parse<Segments>(value.AsSpan(i, 1)))
            .Aggregate((sum, cur) => sum | cur);

    private static int SegmentCount(Segments value) =>
            Enum.GetValues<Segments>().Count(x => value.HasFlag(x));


    [Theory]
    [InlineData("Day08_Example01", 26)]
    [InlineData("Day08_Input", 521)]
    public void Part01(string input, int expectedResult)
    {
        var lines = LoadData(input).Split(Environment.NewLine);
        var sum = 0;

        foreach (var line in lines)
        {
            var outputs = line.Split('|')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ParseSegments);
            sum += outputs.Count(s => SegmentCount(s) switch {
                2 => true, // 1
                3 => true, // 7
                4 => true, // 3
                5 => false,
                6 => false,
                7 => true, // 8
                _ => throw new InvalidDataException($"Unexpected number of segments ({SegmentCount(s)}) for: {s}"),
            });
        }

        Assert.Equal(expectedResult, sum);
    }


    [Theory]
    [InlineData("Day08_Example02", 5353)]
    [InlineData("Day08_Example01", 61229)]
    [InlineData("Day08_Input", 1016804)]
    public void Part02(string input, int expectedResult)
    {
        var lines = LoadData(input).Split(Environment.NewLine);

        var sum = 0;

        foreach (var line in lines)
        {
            var split = line.Split('|');
            var patterns = split[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ParseSegments).ToImmutableArray();
            var output = line.Split('|')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ParseSegments).ToImmutableArray();

            var mapping = patterns.ToDictionary(s => s, s => SegmentCount(s) switch
                {
                    2 => 1,
                    3 => 7,
                    4 => 4,
                    5 => -1,
                    6 => -2,
                    7 => 8,
                    _ => throw new InvalidDataException(),
                });
            Func<int, Segments> reverseLookup = num => mapping.Single(x => x.Value == num).Key;

            var zeroSixNine = mapping.Keys.Where(k => SegmentCount(k) == 6).ToImmutableHashSet();
            var four = reverseLookup(4);
            var nine = zeroSixNine.Single(x => x.HasFlag(four));
            mapping[nine] = 9;
            var zeroSix = zeroSixNine.Remove(nine);
            var one = reverseLookup(1);
            var zero = zeroSix.Single(x => x.HasFlag(one));
            var six = zeroSix.Remove(zero).Single();
            mapping[zero] = 0;
            mapping[six] = 6;

            var twoThreeFive = mapping.Keys.Where(k => SegmentCount(k) == 5).ToImmutableHashSet();
            var three = twoThreeFive.Single(x => x.HasFlag(one));
            mapping[three] = 3;
            var twoFive = twoThreeFive.Remove(three);
            var five = twoFive.Single(x => nine.HasFlag(x));
            var two = twoFive.Remove(five).Single();
            mapping[five] = 5;
            mapping[two] = 2;

            sum += 1000 * mapping[output[0]] + 100 * mapping[output[1]] + 10 * mapping[output[2]] + mapping[output[3]];
        }

        Assert.Equal(expectedResult, sum);
    }
}