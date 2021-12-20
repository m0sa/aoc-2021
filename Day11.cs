namespace aoc_2021;

public class Day11
{
    [Theory]
    [InlineData("Day11_Example02", 2, 9)]
    [InlineData("Day11_Example01", 2, 35)]
    [InlineData("Day11_Example01", 10, 204)]
    [InlineData("Day11_Example01", 100, 1656)]
    [InlineData("Day11_Input", 100, 1739)]
    public void Part01(string input, int steps, int expectedFalshes)
    {
        var (flashed, _) = OctoNav(ParseInput(LoadData(input)), steps);
        Assert.Equal(expectedFalshes, flashed);
    }

    [Theory]
    [InlineData("Day11_Example01", 195)]
    [InlineData("Day11_Input", 324)]
    public void Part02(string input, int expectedStep)
    {
        var (_, steps) = OctoNav(ParseInput(LoadData(input)), 1000);
        Assert.Equal(expectedStep, steps);
    }

    private static int[][] ParseInput(string input) => input.Split(Environment.NewLine).Select(l => l.Select((_, i) => int.Parse(l.AsSpan(i, 1))).ToArray()).ToArray() ?? throw new InvalidDataException();

    private static (int flashed, int steps) OctoNav(int[][] energies, int maxSteps)
    {
        var positions = energies.SelectMany((l, y) => l.Select((_, x) => (x, y))).ToImmutableList();

        static bool BoundCheck(int[][] array, (int x, int y) pos)
            => pos.y >= 0
            && pos.x >= 0 
            && pos.y < array.Length
            && pos.x < array[pos.y].Length;

        static void Increase(int[][] array, (int x, int y) pos)
        {
            if (BoundCheck(array, pos))
            {
                var startValue = array[pos.y][pos.x];
                var endValue = startValue + 1;
                array[pos.y][pos.x] = endValue;
            }
        }

        var flashed = 0;
        var step = 1;
        var flashCache = new HashSet<(int x, int y)>();
        for (; step <= maxSteps; step++)
        {
            flashCache.Clear();
            foreach(var pos in positions)
            {
                Increase(energies, pos);
            }

            for (var anyFlashed = true; anyFlashed; )
            {
                anyFlashed = false;
                foreach(var pos in positions)
                {
                    if (energies[pos.y][pos.x] > 9 && flashCache.Add(pos))
                    {
                        Increase(energies, (pos.x, pos.y+1));
                        Increase(energies, (pos.x, pos.y-1));
                        Increase(energies, (pos.x+1, pos.y));
                        Increase(energies, (pos.x-1, pos.y));
                        Increase(energies, (pos.x+1, pos.y+1));
                        Increase(energies, (pos.x-1, pos.y+1));
                        Increase(energies, (pos.x+1, pos.y-1));
                        Increase(energies, (pos.x-1, pos.y-1));
                        anyFlashed = true;
                    }
                }
            }

            foreach(var pos in flashCache)
            {
                energies[pos.y][pos.x] = 0;
            }

            flashed += flashCache.Count;
            if (flashCache.Count == positions.Count)
            {
                break;
            }
        }
        return (flashed, step);
    }
}