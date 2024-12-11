

// 



// 

// 






using System.Reflection;

using Docker.Registry.DotNet.Domain.QueryStrings;

namespace Docker.Registry.DotNet.Application.QueryStrings;

internal class QueryString : IReadOnlyQueryString
{
    private readonly Dictionary<string, string[]> _values = new();

    public string GetQueryString()
    {
        return string.Join(
            "&",
            this._values.Select(
                pair => string.Join(
                    "&",
                    pair.Value.Select(
                        v => $"{Uri.EscapeUriString(pair.Key)}={Uri.EscapeDataString(v)}"))));
    }

    public void Add(string key, string? value)
    {
        this._values.Add(key, [value ?? string.Empty]);
    }

    public void Add(string key, object? value)
    {
        this._values.Add(key, [value?.ToString() ?? string.Empty]);
    }

    public void Add(string key, string[] values)
    {
        this._values.Add(key, values);
    }

    /// <summary>
    ///     Adds query parameters using reflection. Object must have [QueryParameter] attributes
    ///     on its properties for it to map properly.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    public void AddFromObject<T>(T? instance)
        where T : class
    {
        if (instance == null) return;

        var propertyInfos = instance.GetType().GetProperties();

        foreach (var p in propertyInfos)
        {
            var attribute = p.GetCustomAttribute<QueryParameterAttribute>();
            if (attribute != null)
            {
                // TODO: Maybe switch to FastMember to improve performance here or switch to static delegate generation
                var value = p.GetValue(instance, null);
                if (value != null) this.Add(attribute.Key, value.ToString());
            }
        }
    }
}