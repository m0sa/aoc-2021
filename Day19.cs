namespace aoc_2021;

public class Day19
{
    private static float Deg90 = (float)Math.PI * 0.5f;
    private static float Deg180 = (float)Math.PI;
    private static float Deg270 = (float)Math.PI * 1.5f;

    private static Matrix4x4[] Rotations =
    {
        Matrix4x4.CreateFromYawPitchRoll(0     , 0     , 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg90 , 0     , 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg180, 0     , 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg270, 0     , 0),
        Matrix4x4.CreateFromYawPitchRoll(0     , 0     , Deg90),
        Matrix4x4.CreateFromYawPitchRoll(Deg90 , 0     , Deg90),
        Matrix4x4.CreateFromYawPitchRoll(Deg180, 0     , Deg90),
        Matrix4x4.CreateFromYawPitchRoll(Deg270, 0     , Deg90),
        Matrix4x4.CreateFromYawPitchRoll(0     , 0     , -Deg90),
        Matrix4x4.CreateFromYawPitchRoll(Deg90 , 0     , -Deg90),
        Matrix4x4.CreateFromYawPitchRoll(Deg180, 0     , -Deg90),
        Matrix4x4.CreateFromYawPitchRoll(Deg270, 0     , -Deg90),
        Matrix4x4.CreateFromYawPitchRoll(0     , 0     , Deg180),
        Matrix4x4.CreateFromYawPitchRoll(Deg90 , 0     , Deg180),
        Matrix4x4.CreateFromYawPitchRoll(Deg180, 0     , Deg180),
        Matrix4x4.CreateFromYawPitchRoll(Deg270, 0     , Deg180),
        Matrix4x4.CreateFromYawPitchRoll(0     , Deg90 , 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg90 , Deg90 , 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg180, Deg90 , 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg270, Deg90 , 0),
        Matrix4x4.CreateFromYawPitchRoll(0     , -Deg90, 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg90 , -Deg90, 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg180, -Deg90, 0),
        Matrix4x4.CreateFromYawPitchRoll(Deg270, -Deg90, 0),
    };
    private static (int x, int y, int z) Rotate((int x, int y, int z) vector, int rotationIndex)
    {
        var (x, y, z) = vector;
        var t = Vector3.Transform(new Vector3(x, y, z), Rotations[rotationIndex]);
        return ((int)Math.Round(t.X, 0), (int)Math.Round(t.Y, 0), (int)Math.Round(t.Z, 0));
    }

    [Fact]
    public void EulerSets()
    {
        var vectors = Enumerable.Range(0, Rotations.Length).Select(i => Rotate((1, 2, 3), i)).Distinct().ToArray();
        Assert.Equal(24, vectors.Length);
        Assert.Equal(24, Rotations.Length);
    }

    private static List<ImmutableHashSet<(int x, int y, int z)>> LoadScan(string input)
    {
        var scans = new List<ImmutableHashSet<(int x, int y, int z)>>();
        var beacons = new HashSet<(int x, int y, int z)>();
        foreach(var line in input.Split(Environment.NewLine))
        {
            if(line.StartsWith("--- scanner"))
            {
                beacons.Clear();
            }
            else if (string.IsNullOrWhiteSpace(line))
            {
                scans.Add(beacons.ToImmutableHashSet());
            }
            else
            {
                var c = line.Split(',').Select(int.Parse).ToArray();
                beacons.Add((c[0], c[1], c[2]));
            }
        }
        return scans;
    }

    [Theory]
    [InlineData("Day19_Example01", 12, 79, 3621)]
    [InlineData("Day19_Input", 12, 376, 10772)]
    public void Part1(string input, int requiredOverlap, int expectedBeacons, int expectedDistance)
    {
        var unresolved = LoadScan(LoadData(input));
        var sensors = new List<(int x, int y, int z)>()
        {
            (0, 0, 0)
        };
        var resolved = unresolved[0].ToImmutableHashSet();
        unresolved.RemoveAt(0);

        while (unresolved.Count > 0)
        {
            var found = false;
            for (var i = 0; i < unresolved.Count; i++)
            {
                var scan = unresolved[i];
                var candidates = 
                    from ri in Enumerable.Range(0, Rotations.Length).AsParallel()
                    let rotated = scan.Select(b => Rotate(b, ri)).ToList()
                    from r in rotated
                    from b in resolved.AsParallel()
                    let offset = (x: b.x - r.x, y: b.y - r.y, z: b.z - r.z)
                    let beacons = rotated.Select(r => (r.x + offset.x, r.y + offset.y, r.z + offset.z))
                    where resolved.Intersect(beacons).Count >= requiredOverlap
                    select (offset, beacons);
                var match = candidates.FirstOrDefault();
                if (match.beacons != null)
                {
                    resolved = resolved.Union(match.beacons);
                    sensors.Add(match.offset);
                    unresolved.RemoveAt(i);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Assert.Fail("Couldn't find overlap");
            }
        }

        Assert.Equal(expectedBeacons, resolved.Count);

        // part 2
        var expanse = 
            (from s1 in sensors
            from s2 in sensors.Except(new [] {s1})
            select Math.Abs(s1.x - s2.x) + Math.Abs(s1.y - s2.y) + Math.Abs(s1.z - s2.z))
                .Max();
        Assert.Equal(expectedDistance, expanse);
    }
}