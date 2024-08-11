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

using Docker.Registry.DotNet.Application.QueryStrings;
using Docker.Registry.DotNet.Domain.Blobs;
using Docker.Registry.DotNet.Domain.ImageReferences;

namespace Docker.Registry.DotNet.Application.Endpoints;

internal class BlobUploadOperations(RegistryClient client) : IBlobUploadOperations
{
    public async Task<ResumableUpload> StartUploadBlob(
        string name,
        CancellationToken token = default)
    {
        var path = $"{client.RegistryVersion}/{name}/blobs/uploads/";
        var response = await client.MakeRequest(
            HttpMethod.Post,
            path,
            token: token);

        return new ResumableUpload
        {
            DockerUploadUuid = response.Headers.GetString("Docker-Upload-UUID"),
            Location = response.Headers.GetString("location"),
            Range = response.Headers.GetString("Range")
        };
    }

    public Task<CompletedUploadResponse> MonolithicUploadBlob(
        ResumableUpload resumable,
        ImageDigest digest,
        Stream stream,
        CancellationToken token = default)
    {
        return CompleteBlobUpload(
            resumable,
            digest,
            stream,
            token: token);
    }

    public async Task<MountResponse> MountBlob(
        string name,
        MountParameters parameters,
        CancellationToken token = default)
    {
        var queryString = new QueryString();
        queryString.Add("mount", parameters.Digest);
        queryString.Add("from", parameters.From);

        var response = await client.MakeRequest(
            HttpMethod.Post,
            $"{client.RegistryVersion}/{name}/blobs/uploads/",
            queryString,
            token: token);

        return new MountResponse
        {
            DockerUploadUuid = response.Headers.GetString("Docker-Upload-UUID"),
            Location = response.Headers.GetString("location"),
            Created = response.StatusCode == HttpStatusCode.Created
        };
    }

    public async Task<BlobUploadStatus> GetBlobUploadStatus(
        string name,
        string uuid,
        CancellationToken cancellationToken = default)
    {
        var response = await client.MakeRequest(
            HttpMethod.Get,
            $"{name}/blobs/uploads/{uuid}",
            token: cancellationToken);

        return new BlobUploadStatus
        {
            DockerUploadUuid = response.Headers.GetString("Docker-Upload-UUID"),
            Range = response.Headers.GetString("Range")
        };
    }

    public async Task<ResumableUpload> UploadBlobChunk(
        ResumableUpload resumable,
        Stream chunk,
        long? from = null,
        long? to = null,
        CancellationToken token = default)
    {
        var response = await client.MakeRequest(
            new HttpMethod("PATCH"),
            resumable.Location,
            content: () =>
            {
                chunk.Position = 0;
                var content = new StreamContent(chunk);
                content.Headers.ContentLength = chunk.Length;
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentRange =
                    new ContentRangeHeaderValue(from ?? 0, to ?? chunk.Length);
                return content;
            },
            token: token);

        return new ResumableUpload
        {
            DockerUploadUuid = response.Headers.GetString("Docker-Upload-UUID"),
            Location = response.Headers.GetString("location"),
            Range = response.Headers.GetString("Range")
        };
    }

    public async Task<CompletedUploadResponse> CompleteBlobUpload(
        ResumableUpload resumable,
        ImageDigest digest,
        Stream? chunk = null,
        long? from = null,
        long? to = null,
        CancellationToken token = default)
    {
        var queryString = new QueryString();
        queryString.Add("digest", digest);

        var response = await client.MakeRequest(
            HttpMethod.Put,
            resumable.Location,
            queryString,
            content: () =>
            {
                if (chunk is null) chunk = new MemoryStream();
                chunk.Position = 0;
                var content = new StreamContent(chunk);
                content.Headers.ContentLength = chunk.Length;
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentRange =
                    new ContentRangeHeaderValue(from ?? 0, to ?? chunk.Length);
                return content;
            },
            token: token);

        return new CompletedUploadResponse
        {
            DockerContentDigest = response.Headers.GetString("Docker-Content-Digest"),
            Location = response.Headers.GetString("location")
        };
    }

    public Task CancelBlobUpload(
        string name,
        string uuid,
        CancellationToken token = default)
    {
        var path = $"{client.RegistryVersion}/{name}/blobs/uploads/{uuid}";

        return client.MakeRequest(HttpMethod.Delete, path, token: token);
    }

    /// <summary>
    ///     Perform a monolithic upload.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="contentLength"></param>
    /// <param name="stream"></param>
    /// <param name="digest"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task UploadBlob(
        string name,
        int contentLength,
        Stream stream,
        ImageDigest digest,
        CancellationToken token = default)
    {
        var path = $"{client.RegistryVersion}/{name}/blobs/uploads/";

        var response = await client.MakeRequest(
            HttpMethod.Post,
            path,
            token: token);

        var uuid = response.Headers.GetString("Docker-Upload-UUID");

        Debug.WriteLine($"Uploading with uuid: {uuid}");

        var location = response.Headers.GetString("Location");

        Debug.WriteLine($"Using location: {location}");

        //await GetBlobUploadStatus(name, uuid, cancellationToken);

        try
        {
            using var blobClient = new HttpClient();

            var progressResponse = await blobClient.GetAsync(location, token);

            //Send the contents of the whole file
            var content = new StreamContent(stream);

            content.Headers.ContentLength = stream.Length;
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            content.Headers.ContentRange = new ContentRangeHeaderValue(0, stream.Length);

            var request = new HttpRequestMessage(
                new HttpMethod("PATCH"),
                $"{location}&digest={digest}")
            {
                Content = content
            };

            var response2 = await blobClient.SendAsync(request, token);

            if (response2.StatusCode is < HttpStatusCode.OK or >= HttpStatusCode.BadRequest)
                throw new RegistryApiException(
                    new RegistryApiResponse<string>(
                        response2.StatusCode,
                        null,
                        response.Headers));

            progressResponse = await blobClient.GetAsync(location, token);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Attempting to cancel the upload...");

            await client.MakeRequest(
                HttpMethod.Delete,
                $"{client.RegistryVersion}/{name}/blobs/uploads/{uuid}",
                token: token);

            throw;
        }
    }
}