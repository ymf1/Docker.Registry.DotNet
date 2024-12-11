

// 



// 

// 






using Newtonsoft.Json.Converters;

namespace Docker.Registry.DotNet.Infrastructure.Json;

/// <summary>
///     Facade for <see cref="JsonConvert" />.
/// </summary>
internal class JsonSerializer
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Converters =
        {
            //new JsonIso8601AndUnixEpochDateConverter(),
            //new JsonVersionConverter(),
            new StringEnumConverter(),
            new IsoDateTimeConverter()
            //new TimeSpanSecondsConverter(),
            //new TimeSpanNanosecondsConverter()
        }
    };

    public T? DeserializeObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }

    public string SerializeObject<T>(T value)
    {
        return JsonConvert.SerializeObject(value, Settings);
    }
}