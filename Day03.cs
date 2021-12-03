namespace aoc_2021;

public class Day03
{
    private static int BinToDec(IEnumerable<bool> number) =>
        number.Reverse().Aggregate((val: 0, exp: 1), (s, cur) => (val: cur ? s.val + s.exp : s.val, exp: s.exp * 2), s => s.val);

    [Theory]
    [InlineData("Day03_Example01", 198)]
    [InlineData("Day03_Input", 741950)]
    public void Day03_Part01(string resource, int expectedResult)
    {
        var data = LoadData(resource)
            .Split(Environment.NewLine)
            .Select(x => x.ToCharArray().Select(c => c == '1').ToImmutableArray())
            .ToImmutableArray();
        var len = data[0].Length;
        var epsilon = new bool[len];
        var gamma = new bool[len];

        for (int i = 0; i < len; i++)
        {
            var ones = data.Select(x => x[i]).Count(b => b);
            var zeroes = data.Length - ones;
            epsilon[i] = ones > zeroes;
            gamma[i] = zeroes > ones;
        }

        Assert.Equal(BinToDec(gamma) * BinToDec(epsilon), expectedResult);
    }

    [Theory]
    [InlineData("Day03_Example01", 230)]
    [InlineData("Day03_Input", 903810)]
    public void Day03_Part02(string resource, int expectedResult)
    {
        var data = LoadData(resource)
            .Split(Environment.NewLine)
            .Select(x => x.ToCharArray().Select(c => c == '1').ToImmutableArray())
            .ToImmutableList();

        static ImmutableList<ImmutableArray<bool>> ReduceSet(ImmutableList<ImmutableArray<bool>> remaining, int position, Func<(int ones, int zeroes), bool> nextSetPredicate)
        {
            var ones = remaining.Where(x => x[position] == true).Count();
            var zeroes = remaining.Count - ones;
            var condition = nextSetPredicate((ones, zeroes));
            return remaining.Where(x => x[position] == condition).ToImmutableList();
        }

        var len = data[0].Length;
        var oxySet = data.ToImmutableList();
        var co2Set = data.ToImmutableList();

        for (var i = 0; i < len; i++)
        {
            oxySet = oxySet.Count > 1 ? ReduceSet(oxySet, i, x => x.ones >= x.zeroes) : oxySet;
            co2Set = co2Set.Count > 1 ? ReduceSet(co2Set, i, x => x.ones < x.zeroes) : co2Set;
        }

        Assert.Single(oxySet);
        Assert.Single(co2Set);
        Assert.Equal(expectedResult, BinToDec(oxySet.Single()) * BinToDec(co2Set.Single()));
    }
}