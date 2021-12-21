namespace aoc_2021;

public class Day21
{

    [Theory]
    [InlineData(4, 8, 739785)]
    [InlineData(4, 6, 888735)]
    public void Part1(int player1, int player2, int looserHash)
    {
        static void Increment(IEnumerator<int> what, int by = 1)
        {
            for (var i = 0; i < by; i++)
            {
                what.MoveNext();
            }
        }
        static IEnumerable<int> DeterministicDice(int min, int max)
        {
            for (int i = 0, mod = max - min + 1; true; i++)
            {
                yield return (i % mod) + min;
            }
        }

        var positions = new [] { player1, player2 }.Select(p => DeterministicDice(1, 10).Skip(p - 1).GetEnumerator()).ToArray();
        positions[0].MoveNext();
        positions[1].MoveNext();

        var scores = new [] { 0, 0, 0, 0};
        var dice = DeterministicDice(1, 100).GetEnumerator();

        for(int p = 0; scores[0] < 1000 && scores[1] < 1000; p++)
        {
            var player = p % 2;
            const int dieRolls = 3;
            var pos = positions[player];
            for (var t = 0; t < dieRolls; t++)
            {
                dice.MoveNext();
                Increment(pos, dice.Current);
                scores[2]++;
            }
            scores[player] += pos.Current;
        }

        Assert.Equal(looserHash, scores.Take(2).Single(x => x < 1000) * scores[2]);
    }
}