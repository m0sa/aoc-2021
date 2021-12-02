namespace aoc_2021;

public class Day02
{
    public record struct SubmarineCommand(string command, int units);
    public static Vector2 Dive_Part01(IEnumerable<SubmarineCommand> commands) =>
        commands
            .Aggregate(
                Vector2.Zero,
                (sum, line) => line.command switch
                {
                    "forward" => sum + Vector2.UnitX * line.units,
                    "up" => sum + Vector2.UnitY * -1 * line.units,
                    "down" => sum + Vector2.UnitY * line.units,
                    _ => throw new InvalidOperationException($"Unexpected command {line.command}"),
                });

    public static Vector2 Dive_Part02(IEnumerable<SubmarineCommand> commands) =>
        commands
            .Aggregate(
                (position: Vector2.Zero, aim: 0),
                (state, line) => line.command switch
                {
                    "forward" => (state.position + Vector2.UnitX * line.units + Vector2.UnitY * state.aim * line.units, state.aim),
                    "up" => (state.position, state.aim - line.units),
                    "down" => (state.position, state.aim + line.units),
                    _ => throw new InvalidOperationException($"Unexpected command {line.command}"),
                },
                state => state.position);

    [Theory]
    [InlineData(nameof(Dive_Part01), "Day02_Example01", 150)]
    [InlineData(nameof(Dive_Part01), "Day02_Input", 2322630)]
    [InlineData(nameof(Dive_Part02), "Day02_Example01", 900)]
    [InlineData(nameof(Dive_Part02), "Day02_Input", 2105273490)]
    public void Tests(string method, string resource, int expectedResult)
    {
        var testMethod = typeof(Day02).GetMethod(method, BindingFlags.Public | BindingFlags.Static) ?? throw new ArgumentException($"Method {method} not found", nameof(method));
        var data = LoadData(resource)
            .Split(Environment.NewLine)
            .Select(line => line.Split(Space))
            .Select(lineSplit => new SubmarineCommand(command: lineSplit[0], units: int.Parse(lineSplit[1])));
        var result = (Vector2) (testMethod.Invoke(null, new object[] {data}) ?? throw new InvalidOperationException($"Method {method} returned invalid data"));
        Assert.Equal(expectedResult, (int)result.X * (int)result.Y);
    }
}