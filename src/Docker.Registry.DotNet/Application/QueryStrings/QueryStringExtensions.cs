

// 



// 

// 






namespace Docker.Registry.DotNet.Application.QueryStrings;

internal static class QueryStringExtensions
{
    /// <summary>
    ///     Adds the value to the query string if it's not null.
    /// </summary>
    /// <param name="readOnlyQueryString"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    internal static void AddIfNotNull<T>(this QueryString readOnlyQueryString, string key, T? value)
        where T : struct
    {
        if (value != null) readOnlyQueryString.Add(key, $"{value.Value}");
    }

    /// <summary>
    /// </summary>
    /// <param name="readOnlyQueryString"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    internal static void AddIfNotEmpty(this QueryString readOnlyQueryString, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value)) readOnlyQueryString.Add(key, value);
    }
}