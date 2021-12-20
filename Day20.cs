namespace aoc_2021;

using Image = ImmutableDictionary<(int x, int y), bool>;
using Algorithm = ImmutableArray<bool>;

public class Day20
{
    public static IEnumerable<(int x, int y)> Area9((int x, int y) center)
    {
        var (x, y) = center;
        yield return (x - 1, y - 1);
        yield return (x    , y - 1);
        yield return (x + 1, y - 1);
        yield return (x - 1, y    );
        yield return (x    , y    );
        yield return (x + 1, y    );
        yield return (x - 1, y + 1);
        yield return (x    , y + 1);
        yield return (x + 1, y + 1);
    }

    public static IEnumerable<(int x, int y)> EnumeratePixels(Image i, int by)
    {
        var mm = i.Keys.OrderBy(p => p.y).ThenBy(p => p.x).ToList();
        var min = mm[0];
        var max = mm[^1];
        for (var y = min.y - by; y <= max.y + by; y++)
        {
            for (var x = min.x - by; x <= max.x + by; x++)
            {
                yield return (x, y);
            }
        }
    }

    private static Image Pad(Image image, int by, bool lit) 
        => EnumeratePixels(image, by).ToImmutableDictionary(p => p, p => image.TryGetValue(p, out var l) ? l : lit);
    private static int EnhancedPixelIndex((int x, int y) pixel, Image image, bool infinityLit) =>
        Area9(pixel)
            .Reverse()
            .Select((pixel, index) => (pixel, index))
            .Aggregate(0, (acc, i) => acc + (int)((image.TryGetValue(i.pixel, out var pixelLit) ? pixelLit : infinityLit)  ? Math.Pow(2, i.index) : 0));
    private static Image EnhanceImage(Image image, Algorithm algorithm)
    {
        var infinityKey = image.Keys.OrderBy(p => p.y).ThenBy(p => p.x).First();
        var infinityLit = image[infinityKey];
        var transformed = EnumeratePixels(image, 0)
            .Select(p => (pixel: p, lit: algorithm[EnhancedPixelIndex(p, image, infinityLit)]))
            .ToImmutableDictionary(p => p.pixel, p => p.lit);
        var padded = Pad(transformed, 1, transformed[infinityKey]);
        return padded;
    }

    [Theory]
    [InlineData("Day20_Example01", 2, 35)]
    [InlineData("Day20_Input", 2, 5425)]
    [InlineData("Day20_Example01", 50, 3351)]
    [InlineData("Day20_Input", 50, 14052)]
    public void Test(string input, int steps, int expectedPixels)
    {
        var lines = LoadData(input).Split(Environment.NewLine);
        var algorithm = lines[0].Select(x => x == '#').ToImmutableArray();
        var image = lines
            .Skip(2)
            .SelectMany((line, y) => line.Select((pixel, x) => (pixel: (x, y), lit: pixel == '#')))
            .ToImmutableDictionary(x => x.pixel, x => x.lit);

        var processedImage = Enumerable.Range(0, steps).Aggregate(Pad(image, 5, false), (img, _) => EnhanceImage(img, algorithm));
        var litPixels = processedImage.Where(kvp => kvp.Value).Count();

        Assert.Equal(expectedPixels, litPixels);
    }
}