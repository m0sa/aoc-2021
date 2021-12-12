namespace aoc_2021;
using Map = ImmutableDictionary<string, ImmutableHashSet<string>>;
using Path = ImmutableList<string>;

public class Day12
{
    private static Map Parse(string input)
    {
        var lookup = new Dictionary<string, HashSet<string>>();

        void AddPath(string from, string to) {
            (lookup.TryGetValue(from, out var dst)
                ? dst
                : lookup[from] = new ()).Add(to);
        }
        foreach(var line in input.Split(Environment.NewLine).Select(l => l.Split('-')))
        {
            AddPath(line[0], line[1]);
            AddPath(line[1], line[0]);
        }
        return lookup.ToImmutableDictionary(l => l.Key, l => l.Value.ToImmutableHashSet());
    }

    private const string start = "start";
    private const string end = "end";


    [Theory]
    [InlineData("Day12_Example01", 10)]
    [InlineData("Day12_Example02", 19)]
    [InlineData("Day12_Example03", 226)]
    [InlineData("Day12_Input", 4411)]
    public void Part1(string input, int expectedResult)
    {
        var paths = Traverse(Parse(LoadData(input)), ImmutableList<string>.Empty, start);

        Assert.Equal(expectedResult, paths.Count());
    }

    private static IEnumerable<Path> Traverse(Map map, Path visited, string position)
    {
        var path = visited.Add(position);
        return position == end
            ? new [] { path }
            : map[position]
                .Where(dst => 
                    dst == dst.ToUpperInvariant() // big cave
                    || !path.Contains(dst) // not visited before
                )
                .SelectMany(dst => Traverse(map, path, dst));
    }

    [Theory]
    [InlineData("Day12_Example01", 36)]
    [InlineData("Day12_Input", 136767)]
    public void Part2(string input, int expectedResult)
    {
        var paths = Traverse2(Parse(LoadData(input)), ImmutableList<string>.Empty, start);

        Assert.Equal(expectedResult, paths.Count());
    }

    private static IEnumerable<Path> Traverse2(Map map, Path visited, string position) 
    {
        var path = visited.Add(position);
        return map[position].SelectMany(dst =>
            dst switch {
                string d when d == d.ToUpperInvariant() => Traverse2(map, path, d), // big cave
                start => Enumerable.Empty<Path>(), // not valid
                end => Traverse(map, path, end), // end with regular traversal
                _ => path.Count(x => x == dst) switch {
                    0 => Traverse2(map, path, dst), // visit small cave for 1st time
                    1 => Traverse(map, path, dst), // visit small cave for 2nd time, continue with regular traversal
                    _ => Enumerable.Empty<Path>(), // can't visit cave for 3rd+ time
                },
            });
    }
}