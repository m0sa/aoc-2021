using System.Collections.Concurrent;

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

    [Theory]
    [InlineData(4, 8, 444356092776315)]
    [InlineData(4, 6, 647608359455719)]
    public void Part2(int player1pos, int player2pos, long winningUniverses)
    {
        var result = Q_Memoized(
            cache: new(),
            state: new()
            {
                Player1 = new () { Position = player1pos },
                Player2 = new () { Position = player2pos },
                Player1sTurn = true,
            });

        Assert.Equal(winningUniverses, Math.Max(result.Player1Wins, result.Player2Wins));
    }

    private static ImmutableDictionary<int, int> RollFrequencies = (
            from d3_1 in Enumerable.Range(1, 3)
            from d3_2 in Enumerable.Range(1, 3)
            from d3_3 in Enumerable.Range(1, 3)
            select d3_1 + d3_2 + d3_3)
                .GroupBy(x => x)
                .ToImmutableDictionary(x => x.Key, x => x.Count());
    private record struct PlayerState(int Position, int Score);
    private record struct QuantumGameState(PlayerState Player1, PlayerState Player2, bool Player1sTurn);
    private record struct QuantumGameResult(long Player1Wins, long Player2Wins);
    private static QuantumGameResult Q(QuantumGameState state, Func<QuantumGameState, QuantumGameResult> game)
    {
        if (state.Player1.Score >= 21) return new (1, 0);
        if (state.Player2.Score >= 21) return new (0, 1);

        static PlayerState MoveTo(PlayerState player, int position) => player with { Position = position, Score = player.Score + position };

        var positionNow = state.Player1sTurn ? state.Player1.Position : state.Player2.Position;
        return RollFrequencies.Aggregate(new QuantumGameResult { }, (agg, roll) =>
        {
            var (rollSum, rollFrequency) = roll;
            var positionNext = (positionNow + rollSum - 1) % 10 + 1;
            var stateNext = state.Player1sTurn
                ? state with { Player1 = MoveTo(state.Player1, positionNext), Player1sTurn = false }
                : state with { Player2 = MoveTo(state.Player2, positionNext), Player1sTurn = true };
            var future = game(stateNext);
            return new ()
            {
                Player1Wins = agg.Player1Wins + future.Player1Wins * rollFrequency,
                Player2Wins = agg.Player2Wins + future.Player2Wins * rollFrequency,
            };
        });
    }

    private static QuantumGameResult Q_Memoized(QuantumGameState state, ConcurrentDictionary<QuantumGameState, QuantumGameResult> cache)
        => cache.GetOrAdd(
            key: state,
            factoryArgument: cache,
            valueFactory: (s, c) => Q(s, fs => Q_Memoized(fs, c)));

}