using System.Text;

namespace aoc_2021;

public class Day18
{
    private enum Item
    {
        Value,
        Open,
        Close,
    }

    private static List<(Item type, int value)> Parse(string line) =>
        line
            .Where(c => c != ',')
            .Select(c => c switch {
                '[' => (Item.Open, 0),
                ']' => (Item.Close, 0),
                _ => (Item.Value, c - '0')
            })
            .ToList();

    private static (Item type, int value) Add((Item type, int value) a, (Item type, int value) b)
        => (Item.Value, a.value + b.value);

    private static void Explode(List<(Item type, int value)> number, int i)
    {
        var leftIndex = number.GetRange(0, i).FindLastIndex(t => t.type == Item.Value);
        if (leftIndex == -1)
        {
            number[i+1] = (Item.Value, 0);
        }
        else
        {
            number[leftIndex] = Add(number[leftIndex], number[i+1]);
        }

        var rightIndex = number.FindIndex(i+4, t => t.type == Item.Value);
        if (rightIndex == -1)
        {
            number[i+2] = (Item.Value, 0);
        }
        else
        {
            number[rightIndex] = Add(number[rightIndex], number[i+2]);
        }

        number[i] = (Item.Value, 0);
        number.RemoveRange(i+1, 3);
    }

    private static void Split(List<(Item type, int value)> number, int i)
    {
        var div = number[i].value / 2f;
        number[i] = (Item.Close, 0);
        number.Insert(i, (Item.Value, (int)Math.Ceiling(div)));
        number.Insert(i, (Item.Value, (int)Math.Floor(div)));
        number.Insert(i, (Item.Open, 0));
    }
    

    private static List<(Item type, int value)> Reduce(List<(Item type, int value)> number)
    {
        while (true)
        {
            var openCount = 0;
            var explodeAt = number.FindIndex(x => 
                (openCount = x switch { 
                    (Item.Open, _) => openCount + 1,
                    (Item.Close, _) => openCount - 1,
                    _ => openCount,
                }) >= 5);
            if (explodeAt != - 1)
            {
                Explode(number, explodeAt);
                continue;
            }

            var splitAt = number.FindIndex(x => x.type == Item.Value && x.value > 9);
            if (splitAt != -1)
            {
                Split(number, splitAt);
                continue;
            }

            return number;
        }
    }

    [Theory]
    [InlineData("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
    [InlineData("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
    [InlineData("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
    [InlineData("[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]", "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]")]
    public void ReduceTests(string input, string output)
    {
        var expected = (Parse(output));
        var reduced = (Reduce(Parse(input)));
        Assert.Equal(expected, reduced);
    }

    private static List<(Item type, int value)> Add(List<(Item type, int value)> a, List<(Item type, int value)> b)
    {        
        var sum = new List<(Item type, int value)>(2 + a.Count + b.Count);
        
        sum.Add((Item.Open, 0));
        sum.AddRange(a);
        sum.AddRange(b);
        sum.Add((Item.Close, 0));

        return Reduce(sum);
    }

    private static int Magnitude(IEnumerable<(Item type, int value)> sum)
    {
        var result = sum.ToList();
        while (result.Count > 1)
        {
            for (var i = 0; i < result.Count - 2; i++)
            {
                var (c, _) = result[i];
                if (c == Item.Value && result[i+1].type == Item.Value)
                {
                    var m = result[i].value * 3 + result[i+1].value * 2;
                    result[i] = (Item.Value, m);                   
                    result.RemoveRange(i+1, 2);
                    result.RemoveAt(i-1);
                    break;
                }
            }
        }

        return result.Single().value;
    }

    [Theory]
    [InlineData("Day18_Example01", 4140, 3993)]
    [InlineData("Day18_Input", 4116, 4638)]
    public void Test(string input, int expectedTotal, int expectedMax2)
    {
        var numbers = LoadData(input)
            .Split(Environment.NewLine)
            .Select(Parse)
            .ToArray();

        var totalMagnitude = Magnitude(numbers.Skip(1).Aggregate(numbers[0], (acc, l) => Add(acc, l)));

        Assert.Equal(expectedTotal, totalMagnitude);

        var pairwiseMagnitudes = 
            from f1 in Enumerable.Range(0, numbers.Length)
            from f2 in Enumerable.Range(0, numbers.Length)
            where f1 != f2
            let sum = Add(numbers[f1], numbers[f2])
            select Magnitude(sum);
        var max2Magnitude = pairwiseMagnitudes.Max();

        Assert.Equal(expectedMax2, max2Magnitude);
    }
 }