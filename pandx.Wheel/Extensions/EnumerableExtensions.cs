namespace pandx.Wheel.Extensions;

public static class EnumerableExtensions
{
    public static string JoinAsString(this IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source);
    }
}