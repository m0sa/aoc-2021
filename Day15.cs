namespace aoc_2021;


public class Day15
{

    static IEnumerable<(int x, int y)> StraightNeighbours((int x, int y) point)
    {
        yield return (point.x + 1, point.y);
        yield return (point.x - 1, point.y);
        yield return (point.x, point.y + 1);
        yield return (point.x, point.y - 1);
    }

    static int OneToNine(int num) =>  ((num - 1) % 9) + 1;  

    [Theory]
    [InlineData("Day15_Example01", 1, 40)]
    [InlineData("Day15_Input", 1, 698)]
    [InlineData("Day15_Example02", 5, 37)]
    [InlineData("Day15_Example01", 5, 315)]
    [InlineData("Day15_Input", 5, 3022)]
    public void Test(string input, int fragmentMultiple, long expectedResult)
    {
        var lines = LoadData(input).Split(Environment.NewLine);

        var riskMap = ( 
            from y in Enumerable.Range(0, fragmentMultiple * lines.Length)
            from x in Enumerable.Range(0, fragmentMultiple * lines.Length)
            let line = lines[y % lines.Length]
            let risk = int.Parse(line.AsSpan(x % lines.Length, 1))
            let dx = x / lines.Length
            let dy = y / lines.Length
            select (x, y, risk: OneToNine(risk + dx + dy))
        ).ToImmutableDictionary(p => (p.x, p.y), p => p.risk);
        var source = (0, 0);
        var destination = (x: riskMap.Keys.Max(k => k.x), y: riskMap.Keys.Max(k => k.y));

        var dist = riskMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Key == source ? 0 : long.MaxValue);
        var Q = new PriorityQueue<(int x, int y), long>(dist.Select(kvp => (Element: kvp.Key, Priority: kvp.Value)));
        var visited = new HashSet<(int x, int y)>();
        while (Q.Count > 0)
        {
            var u = Q.Dequeue(); 
            if (!visited.Add(u)) continue;

            foreach(var v in StraightNeighbours(u).Where(riskMap.ContainsKey))
            {
                var alt = dist[u] + riskMap[v];
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    // NOTE: this doesn't update the priority of v, it enqueues it again, so we'll get v in Dequeue multiple times
                    Q.Enqueue(v, alt);
                }
            }
        }

        Assert.Equal(expectedResult, dist[destination]);
    }
}