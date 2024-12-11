

// 



// 

// 









// 



// 

// 






using Docker.Registry.DotNet.Domain.Blobs;

namespace Docker.Registry.DotNet.Application.Endpoints;

internal class BlobOperations(RegistryClient client) : IBlobOperations
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