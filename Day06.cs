using System.Buffers;

namespace aoc_2021;

public class Day06
{
    [Theory]
    [InlineData("Day06_Example01", 1, 5)]
    [InlineData("Day06_Example01", 2, 6)]
    [InlineData("Day06_Example01", 3, 7)]
    [InlineData("Day06_Example01", 4, 9)]
    [InlineData("Day06_Example01", 18, 26)]
    [InlineData("Day06_Example01", 80, 5934)]
    [InlineData("Day06_Input", 80, 343441)]
    [InlineData("Day06_Example01", 256, 26984457539)]
    [InlineData("Day06_Input", 256, 1569108373832)]
    public void Part1(string intput, int days, ulong expectedCount)
    {
        var generation = LoadData(intput).Split(',')
            .Select(ulong.Parse)
            .Aggregate(new ulong[9], (state, x) => {
                state[x] = state[x] + 1u;
                return state;
            }, x => x);

        for (var d = 0; d < days; d++)
        {
            var newGeneration = ArrayPool<ulong>.Shared.Rent(9);
            newGeneration[8] = generation[0];
            newGeneration[7] = generation[8];
            newGeneration[6] = generation[7] + generation[0];
            newGeneration[5] = generation[6];
            newGeneration[4] = generation[5];
            newGeneration[3] = generation[4];
            newGeneration[2] = generation[3];
            newGeneration[1] = generation[2];
            newGeneration[0] = generation[1];
            Array.Copy(newGeneration, generation, 9);
            ArrayPool<ulong>.Shared.Return(newGeneration);
        }
        Assert.Equal(expectedCount, generation.Aggregate(0ul, (sum, c) => sum + c));
    }
}