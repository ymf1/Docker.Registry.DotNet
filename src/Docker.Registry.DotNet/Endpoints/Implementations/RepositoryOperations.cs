// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
//  and Docker.Registry.DotNet Contributors
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using Newtonsoft.Json.Converters;

namespace Docker.Registry.DotNet.Endpoints.Implementations;

public interface IRepositoryOperations
{
    Task<ListRepositoryTagsResponse> ListRepositoryTags(
        string @namespace,
        string repository,
        RepositoryTagsParameters? parameters = null,
        CancellationToken token = default);
}

internal class RepositoryOperations(NetworkClient client) : IRepositoryOperations
{
    public async Task<ListRepositoryTagsResponse> ListRepositoryTags(
        string @namespace,
        string repository,
        RepositoryTagsParameters? parameters = null,
        CancellationToken token = default)
    {
        var queryString = new QueryString();
        queryString.AddFromObject(parameters ?? new RepositoryTagsParameters());

        var response = await client.MakeRequest(
            HttpMethod.Get,
            $"{client.RegistryVersion}/namespaces/{@namespace}/repositories/{repository}/tags",
            queryString,
            token: token);

        return client.JsonSerializer.DeserializeObject<ListRepositoryTagsResponse>(response.Body);
    }
}

[PublicAPI]
public class RepositoryTagsParameters
{
    /// <summary>
    ///     Current page.
    /// </summary>
    [QueryParameter("page")]
    public int Page { get; set; } = 1;

    /// <summary>
    ///     Page Size -- max is 100
    /// </summary>
    [QueryParameter("page_size")]
    public int PageSize { get; set; } = 10;
}

public class ListRepositoryTagsResponse
{
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("next")]
    public string Next { get; set; }

    [JsonProperty("previous")]
    public object Previous { get; set; }

    [JsonProperty("results")]
    public List<RepositoryTag> Tags { get; set; }
}

public class RepositoryTag
{
    [JsonProperty("creator")]
    public int Creator { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("images")]
    public List<TagImage> Images { get; set; }

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

public class TagImage
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