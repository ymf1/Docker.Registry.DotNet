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

using Docker.Registry.DotNet.Domain.ImageReferences;
using Docker.Registry.DotNet.Domain.Manifests;

namespace Docker.Registry.DotNet.Application.Endpoints;

internal class ManifestOperations(RegistryClient client) : IManifestOperations
{
    static readonly IReadOnlyDictionary<string, string> _manifestHeaders = new Dictionary<string, string>
    {
        {
            "Accept",
            $"{ManifestMediaTypes.ManifestSchema1}, {ManifestMediaTypes.ManifestSchema2}, {
                ManifestMediaTypes.ManifestList}, {ManifestMediaTypes.ManifestSchema1Signed}"
        }
    };

    public async Task<GetImageManifestResult> GetManifest(
        string name,
        ImageReference reference,
        CancellationToken token = default)
    {
        ImageDigest? digestReference = null;

        if (reference.IsTag)
        {
            digestReference = await GetDigest(name, reference.Tag!, token);
        }
        else if (reference.IsDigest)
        {
            digestReference = reference.Digest;
        }

        if (digestReference == null)
        {
            throw new ArgumentNullException(
                nameof(digestReference),
                @$"Failed getting the digest reference for ""{reference}""");
        }

        var response = await MakeManifestRequest(name, digestReference.ToReference(), token);

        var contentType = GetContentType(response.GetHeader("ContentType"), response.Body);

        switch (contentType)
        {
            case ManifestMediaTypes.ManifestSchema1:
            case ManifestMediaTypes.ManifestSchema1Signed:
                return new GetImageManifestResult(
                    contentType,
                    client.JsonSerializer.DeserializeObject<ImageManifest2_1>(response.Body),
                    response.Body)
                {
                    DockerContentDigest = response.GetHeader("Docker-Content-Digest"),
                    Etag = response.GetHeader("Etag")
                };

            case ManifestMediaTypes.ManifestSchema2:
                return new GetImageManifestResult(
                    contentType,
                    client.JsonSerializer.DeserializeObject<ImageManifest2_2>(response.Body),
                    response.Body)
                { DockerContentDigest = response.GetHeader("Docker-Content-Digest") };

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
        ImageReference reference,
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
                var content = new StringContent(client.JsonSerializer.SerializeObject(manifest));
                content.Headers.ContentType = new MediaTypeHeaderValue(manifestMediaType);
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

    public async Task DeleteManifest(string name, ImageReference reference, CancellationToken token = default)
    {
        var path = $"{client.RegistryVersion}/{name}/manifests/{reference}";

        await client.MakeRequest(HttpMethod.Delete, path, token: token);
    }

    [PublicAPI]
    public async Task<string> GetManifestRaw(
        string name,
        ImageReference reference,
        CancellationToken token)
    {
        var response = await MakeManifestRequest(name, reference, token);

        return response.Body;
    }

    public async Task<ImageDigest?> GetDigest(string name, ImageTag tag, CancellationToken token = default)
    {
        var response = await MakeManifestRequest(name, tag.ToReference(), token);

        var digestValue = response.GetHeader("Docker-Content-Digest");

        return ImageDigest.TryCreate(digestValue, out var digest) ? digest : null;
    }

    private async Task<RegistryApiResponse<string>> MakeManifestRequest(
        string name,
        ImageReference reference,
        CancellationToken token)
    {
        return await client.MakeRequest(
            HttpMethod.Get,
            $"{client.RegistryVersion}/{name}/manifests/{reference}",
            null,
            _manifestHeaders,
            token: token);
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

        throw new Exception($"Unable to determine schema type from version {check.SchemaVersion}");
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