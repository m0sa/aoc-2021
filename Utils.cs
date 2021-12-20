using System.Reflection;
using System.Runtime.CompilerServices;

namespace aoc_2021;

public static class Utils
{
    public static string LoadData(string? customResourceName = null, [CallerMemberName] string? callerMemberName = null)
    {
        var resourceName = nameof(aoc_2021) + ".Data." + (customResourceName ?? callerMemberName) + ".txt";
        var assembly = Assembly.GetExecutingAssembly();
        using var resourceStream
            = assembly.GetManifestResourceStream(resourceName)
            ?? throw new ArgumentException($"{resourceName} not found, available: { string.Join(",", assembly.GetManifestResourceNames()) }");
        using var reader = new StreamReader(resourceStream);
        return reader.ReadToEnd();
    }

    public static readonly char[] Space = { ' ' };

    public static bool TryReadNumber(IEnumerator<bool> bitStream, long numberOfBits, out long value)
    {
        value = default;
        var bitsRead = ReadBits(bitStream, numberOfBits).ToList();
        if (bitsRead.Count != numberOfBits) return false;

        using var bitsReadStream = bitsRead.GetEnumerator();
        value = ReadNumber(bitsReadStream, numberOfBits);
        return true;
    }

    public static long ReadNumber(IEnumerator<bool> bitStream, long numberOfBits) =>
        ReadBits(bitStream, numberOfBits)
            .Select((bit, index) => bit ? 1L << (int)(numberOfBits - index - 1) : 0L)
            .Sum();

    public static IEnumerable<bool> ReadBits(IEnumerator<bool> bitStream, long numberOfBits)
    {
        while (numberOfBits-- > 0 && bitStream.MoveNext())
        {
            yield return bitStream.Current;
        }
    }
}