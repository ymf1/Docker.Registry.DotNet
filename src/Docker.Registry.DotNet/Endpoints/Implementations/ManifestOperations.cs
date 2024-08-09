//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
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

namespace Docker.Registry.DotNet.Endpoints.Implementations;

internal class ManifestOperations(NetworkClient client) : IManifestOperations
{
    public async Task<GetImageManifestResult> GetManifest(
        string name,
        string reference,
        CancellationToken token = default)
    {
        var headers = new Dictionary<string, string>
        {
            {
                "Accept",
                $"{ManifestMediaTypes.ManifestSchema1}, {ManifestMediaTypes.ManifestSchema2}, {ManifestMediaTypes.ManifestList}, {ManifestMediaTypes.ManifestSchema1Signed}"
            }
        };

        var response = await client.MakeRequest(
            HttpMethod.Head,
            $"{client.RegistryVersion}/{name}/manifests/{reference}",
            null,
            headers,
            token: token);

        var digestReference = response.GetHeader("Docker-Content-Digest");

        response = await client.MakeRequest(
            HttpMethod.Get,
            $"{client.RegistryVersion}/{name}/manifests/{digestReference}",
            null,
            headers,
            token: token);

        var contentType = this.GetContentType(response.GetHeader("ContentType"), response.Body);

        switch (contentType)
        {
            case ManifestMediaTypes.ManifestSchema1:
            case ManifestMediaTypes.ManifestSchema1Signed:
                return new GetImageManifestResult(
                    contentType,
                    client.JsonSerializer.DeserializeObject<ImageManifest2_1>(
                        response.Body),
                    response.Body)
                {
                    DockerContentDigest = response.GetHeader("Docker-Content-Digest"),
                    Etag = response.GetHeader("Etag")
                };

            case ManifestMediaTypes.ManifestSchema2:
                return new GetImageManifestResult(
                    contentType,
                    client.JsonSerializer.DeserializeObject<ImageManifest2_2>(
                        response.Body),
                    response.Body)
                {
                    DockerContentDigest = response.GetHeader("Docker-Content-Digest")
                };

            case ManifestMediaTypes.ManifestList:
                return new GetImageManifestResult(
                    contentType,
                    client.JsonSerializer.DeserializeObject<ManifestList>(response.Body),
                    response.Body);

            default:
                throw new Exception($"Unexpected ContentType '{contentType}'.");
        }
    }

    public async Task<PushManifestResponse> PutManifest(
        string name,
        string reference,
        ImageManifest manifest,
        CancellationToken token)
    {
        string? manifestMediaType = null;
        if (manifest is ImageManifest2_1)
            manifestMediaType = ManifestMediaTypes.ManifestSchema1;
        if (manifest is ImageManifest2_2)
            manifestMediaType = ManifestMediaTypes.ManifestSchema2;
        if (manifest is ManifestList)
            manifestMediaType = ManifestMediaTypes.ManifestList;

        var response = await client.MakeRequest(
            HttpMethod.Put,
            $"{client.RegistryVersion}/{name}/manifests/{reference}",
            content: () =>
            {
                var content = new StringContent(
                    client.JsonSerializer.SerializeObject(manifest));
                content.Headers.ContentType =
                    new MediaTypeHeaderValue(manifestMediaType);
                return content;
            },
            token: token);

        return new PushManifestResponse
        {
            DockerContentDigest = response.GetHeader("Docker-Content-Digest"),
            ContentLength = response.GetHeader("Content-Length"),
            Location = response.GetHeader("location")
        };
    }

    public async Task DeleteManifest(
        string name,
        string reference,
        CancellationToken token = default)
    {
        var path = $"{client.RegistryVersion}/{name}/manifests/{reference}";

        await client.MakeRequest(HttpMethod.Delete, path, token: token);
    }

    [PublicAPI]
    public async Task<string?> GetManifestRaw(
        string name,
        string reference,
        CancellationToken token)
    {
        var headers = new Dictionary<string, string>
        {
            {
                "Accept",
                $"{ManifestMediaTypes.ManifestSchema1}, {ManifestMediaTypes.ManifestSchema2}, {ManifestMediaTypes.ManifestList}, {ManifestMediaTypes.ManifestSchema1Signed}"
            }
        };

        var response = await client.MakeRequest(
            HttpMethod.Get,
            $"{client.RegistryVersion}/{name}/manifests/{reference}",
            null,
            headers,
            token: token);

        return response.Body;
    }

    private string GetContentType(string contentTypeHeader, string manifest)
    {
        if (!string.IsNullOrWhiteSpace(contentTypeHeader))
            return contentTypeHeader;

        var check = JsonConvert.DeserializeObject<SchemaCheck>(manifest);

        if (!string.IsNullOrWhiteSpace(check.MediaType))
            return check.MediaType;

        if (check.SchemaVersion == null)
            return ManifestMediaTypes.ManifestSchema1;

        if (check.SchemaVersion.Value == 2)
            return ManifestMediaTypes.ManifestSchema2;

        throw new Exception(
            $"Unable to determine schema type from version {check.SchemaVersion}");
    }

    private class SchemaCheck
    {
        /// <summary>
        ///     This field specifies the image manifest schema version as an integer.
        /// </summary>
        [DataMember(Name = "schemaVersion")]
        public int? SchemaVersion { get; set; }

        [DataMember(Name = "mediaType")]
        public string? MediaType { get; set; }
    }
}