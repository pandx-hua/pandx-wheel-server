namespace pandx.Wheel.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return (dictionary.TryGetValue(key, out var obj) ? obj : default)!;
    }
}