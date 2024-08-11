using Newtonsoft.Json.Converters;

namespace Docker.Registry.DotNet.Domain.Repository;

public class RepositoryTagImage
{
    [JsonProperty("architecture")]
    public string Architecture { get; set; }

    [JsonProperty("features")]
    public string Features { get; set; }

    [JsonProperty("variant")]
    public object Variant { get; set; }

    [JsonProperty("digest")]
    public string Digest { get; set; }

    [JsonProperty("os")]
    public string Os { get; set; }

    [JsonProperty("os_features")]
    public string OsFeatures { get; set; }

    [JsonProperty("os_version")]
    public object OsVersion { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("last_pulled")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime LastPulled { get; set; }

    [JsonProperty("last_pushed")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime LastPushed { get; set; }
}