namespace aoc_2021;

public class Day17
{
    private static IEnumerable<(int x, int y)> Launch((int x, int y) velocity)
    {
        var position = (x: 0, y: 0);
        while (true)
        {
            position = (position.x + velocity.x, position.y + velocity.y);
            yield return position;
            velocity =
            (
                x: velocity.x switch {
                    0 => 0,
                    int p when p > 0 => p - 1,
                    int n when n < 0 => n + 1,
                    _ => throw new InvalidDataException(),
                },
                y: velocity.y - 1
            );
        }
    }

    [Theory]
    [InlineData(-10, -5, 45)]
    [InlineData(-101, -57, 5050)]
    public void Part1(int y1, int y2, int highestPossiblePosition)
    {
        var lowerBound = Math.Min(y1, y2);
        var upperBound = Math.Max(y1, y2);
        var result = Enumerable
            .Range(lowerBound, Math.Abs(lowerBound) * 2)
            .Select(vy => (x: 0, y: vy))
            .Select(v => Launch(v)
                .TakeWhile(p => p.y >= lowerBound)
                .Aggregate(
                    (targetAreaHit: false, maxY: 0),
                    (acc, p) => (
                        acc.targetAreaHit || (lowerBound >= p.y && p.y <= upperBound),
                        Math.Max(acc.maxY, p.y)
                    )
                ))
            .Where(acc => acc.targetAreaHit)
            .Select(acc => acc.maxY)
            .Max();

        Assert.Equal(highestPossiblePosition, result);
    }

    [Theory]
    [InlineData(20, 30, -10, -5, 112)]
    [InlineData(257, 286, -101, -57, 2223)]
    public void Part2(int x1, int x2, int y1, int y2, int numberOfVelocities)
    {
        var xlo = Math.Min(x1, x2);
        var xhi = Math.Max(x1, x2);
        var ylo = Math.Min(y1, y2);
        var yhi = Math.Max(y1, y2);
        var velocities =
            from vx in Enumerable.Range(1, xhi)
            from vy in Enumerable.Range(ylo, Math.Abs(ylo) * 2)
            let velocity = (x: vx, y: vy)
            let points = Launch(velocity).TakeWhile(p => p.y >= ylo && p.x <= xhi)
            where points.Any(p => p.x >= xlo && p.x <= xhi && p.y >= ylo && p.y <= yhi)
            select velocity;

        Assert.Equal(numberOfVelocities, velocities.Count());
    }
}