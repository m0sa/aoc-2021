namespace aoc_2021;

public class Day04
{
    public record BingoBoard(ImmutableHashSet<int> RemainingNumbers, ImmutableList<ImmutableHashSet<int>> Runs);

    public record BingoGame(IEnumerable<int> DrawnNumbers, ImmutableList<BingoBoard> Boards, int LastDrawn = 0);

    const int BoardDimension = 5;

    public static BingoGame ParseBingoGame(string input)
    {
        var split = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var numbers = split[0].Split(',').Select(int.Parse).ToImmutableList();

        var boardBuilder = new List<List<int>>();
        var boards = new List<BingoBoard>();
        foreach (var line in split.Skip(1))
        {
            boardBuilder.Add(line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList());
            if (boardBuilder.Count == BoardDimension)
            {
                var board = new BingoBoard(
                    boardBuilder.SelectMany(x => x).ToImmutableHashSet(),
                    Enumerable.Range(0, BoardDimension).SelectMany(i => new []
                    {
                        Enumerable.Range(0, BoardDimension).Select(j => boardBuilder[i][j]).ToImmutableHashSet(),
                        Enumerable.Range(0, BoardDimension).Select(j => boardBuilder[j][i]).ToImmutableHashSet(),
                    }).ToImmutableList()
                );
                boards.Add(board);
                boardBuilder.Clear();
            }
        }
        return new BingoGame(numbers, boards.ToImmutableList());
    }

    [Fact]
    public void ParseTest()
    {
        var input = LoadData("Day04_Example");
        var game = ParseBingoGame(input);

        Assert.Equal(3, game.Boards.Count);
        Assert.Equal(27, game.DrawnNumbers.Count());
    }

    [Theory]
    [InlineData("Day04_Example", 4512)]
    [InlineData("Day04_Input", 67716)]
    public void Part1(string input, int expectedResult)
    {
        var game = ParseBingoGame(LoadData(input));
        var boards = game.Boards;
        BingoBoard? winner = null;
        int? winningNumber = null;
        foreach (var drawn in game.DrawnNumbers)
        {
            boards = boards
                .Select(b => new BingoBoard(
                    b.RemainingNumbers.Remove(drawn),
                    b.Runs.Select(r => r.Remove(drawn)).ToImmutableList()))
                .ToImmutableList();

            winner = boards.SingleOrDefault(b => b.Runs.Any(r => r.IsEmpty));
            if (winner != null)
            {
                winningNumber = drawn;
                break;
            }
        }

        Assert.NotNull(winner);
        Assert.NotNull(winningNumber);
        var remaining = winner!.RemainingNumbers.Sum();

        Assert.Equal(expectedResult, remaining * winningNumber);
    }
}

