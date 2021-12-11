namespace aoc_2021;

public class Day09
{
    [Theory]
    [InlineData("Day09_Example01", 15, 1134)]
    [InlineData("Day09_Input", 494, 1048128)]
    public void Test(string input, int expectedLowPoint, int expectedBaisin)
    {
        var heightMap = LoadData(input).Split(Environment.NewLine).Select(line=>line.Select((_,i) => int.Parse(line.AsSpan(i,1))).ToArray()).ToArray();
        var positions = heightMap.SelectMany((l,y) => l.Select((_, x) => (x, y))).ToImmutableHashSet();
        static IEnumerable<(int x, int y)> Neighbours ((int x, int y) position)
        {
            yield return (position.x + 1, position.y);
            yield return (position.x - 1, position.y);
            yield return (position.x, position.y + 1);
            yield return (position.x, position.y - 1);
        }

        // part 1
        HashSet<(int x, int y)> lowPoints = new ();
        foreach(var pos in positions)
        {
            var height = heightMap[pos.y][pos.x];
            var isLowPoint = Neighbours(pos)
                .Where(positions.Contains)
                .All(p => heightMap[p.y][p.x] > height);
            if (isLowPoint)
            {
                lowPoints.Add(pos);
            }
        }
        var lowPointResult = lowPoints.Sum(pos => heightMap[pos.y][pos.x] + 1);
        Assert.Equal(expectedLowPoint, lowPointResult);

        // part 2
        var baisins = lowPoints.Select(lpos =>
        {
            var baisin = new HashSet<(int x, int y)>() { lpos };            
            var height = heightMap[lpos.y][lpos.x] + 1;
            while (height < 9)
            {
                var newHeightNeighbours = baisin
                    .SelectMany(Neighbours)
                    .Where(positions.Contains)
                    .Where(p => heightMap[p.y][p.x] == height)
                    .ToHashSet();
                if (newHeightNeighbours.Count == 0)
                {
                    break;
                }
                baisin.UnionWith(newHeightNeighbours);
                height ++;
            }
            return baisin;
        }).ToList();
        var baisinResult = baisins
            .OrderByDescending(b => b.Count)
            .Take(3)
            .Aggregate(1, (res, b) => res *= b.Count);
        Assert.Equal(expectedBaisin, baisinResult);
    }
}