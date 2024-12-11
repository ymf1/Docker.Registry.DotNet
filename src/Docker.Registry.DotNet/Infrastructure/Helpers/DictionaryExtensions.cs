

// 



// 

// 






namespace Docker.Registry.DotNet.Infrastructure.Helpers;

internal static class DictionaryExtensions
{
    public static string GetQueryString(this IDictionary<string, string[]> values)
    {
        return string.Join(
            "&",
            values.Select(
                pair => string.Join(
                    "&",
                    pair.Value.Select(
                        v => $"{Uri.EscapeUriString(pair.Key)}={Uri.EscapeDataString(v)}"))));
    }

    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        TKey key)
    {
        return dict.TryGetValue(key, out var value) ? value : default;
    }
}