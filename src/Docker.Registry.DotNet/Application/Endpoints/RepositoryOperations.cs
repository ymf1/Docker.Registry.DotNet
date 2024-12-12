
using Docker.Registry.DotNet.Application.QueryStrings;
using Docker.Registry.DotNet.Domain.Repository;

namespace Docker.Registry.DotNet.Application.Endpoints;

internal class RepositoryOperations(RegistryClient client) : IRepositoryOperations
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