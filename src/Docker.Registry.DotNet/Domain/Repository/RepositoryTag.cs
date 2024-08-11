using Newtonsoft.Json.Converters;

namespace Docker.Registry.DotNet.Domain.Repository;

public class RepositoryTag
{
    [JsonProperty("creator")]
    public int Creator { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("images")]
    public List<RepositoryTagImage> Images { get; set; }

    [JsonProperty("last_updated")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime LastUpdated { get; set; }

    [JsonProperty("last_updater")]
    public int LastUpdater { get; set; }

    [JsonProperty("last_updater_username")]
    public string LastUpdaterUsername { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("repository")]
    public int Repository { get; set; }

    [JsonProperty("full_size")]
    public long FullSize { get; set; }

    [JsonProperty("v2")]
    public bool V2 { get; set; }

    [JsonProperty("tag_status")]
    public string TagStatus { get; set; }

    [JsonProperty("tag_last_pulled")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime TagLastPulled { get; set; }

    [JsonProperty("tag_last_pushed")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime TagLastPushed { get; set; }

    [JsonProperty("media_type")]
    public string MediaType { get; set; }

    [JsonProperty("content_type")]
    public string ContentType { get; set; }

    [JsonProperty("digest")]
    public string Digest { get; set; }
}