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
}