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

internal class BlobOperations(NetworkClient client) : IBlobOperations
{
    public async Task<GetBlobResponse> GetBlob(
        string name,
        string digest,
        CancellationToken token = default)
    {
        var response = await client.MakeRequestForStreamedResponseAsync(
            HttpMethod.Get,
            $"{client.RegistryVersion}/{name}/blobs/{digest}",
            token: token);

        return new GetBlobResponse(
            response.Headers.GetString("Docker-Content-Digest"),
            response.Body);
    }

    public Task DeleteBlob(
        string name,
        string digest,
        CancellationToken token = default)
    {
        return client.MakeRequest(
            HttpMethod.Delete,
            $"{client.RegistryVersion}/{name}/blobs/{digest}",
            token: token);
    }

    public async Task<bool> BlobExists(
        string name,
        string digest,
        CancellationToken token = default)
    {
        var path = $"{client.RegistryVersion}/{name}/blobs/{digest}";

        var response = await client.MakeRequestNotErrorAsync(
            HttpMethod.Head,
            path,
            token: token);

        if (response.StatusCode != HttpStatusCode.NotFound)
            client.HandleIfErrorResponse(response);

        return response.StatusCode == HttpStatusCode.OK;
    }
}