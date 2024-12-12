

using Docker.Registry.DotNet.Application.QueryStrings;
using Docker.Registry.DotNet.Domain.Catalogs;

namespace Docker.Registry.DotNet.Application.Endpoints;

internal class CatalogOperations(RegistryClient client) : ICatalogOperations
{
    public async Task<CatalogResponse> GetCatalog(
        CatalogParameters? parameters = null,
        CancellationToken token = default)
    {

        parameters ??= new CatalogParameters();

        var queryParameters = new QueryString();

        queryParameters.AddFromObject(parameters);

        var response = await client.MakeRequest(
            HttpMethod.Get,
            $"{client.RegistryVersion}/_catalog",
            queryParameters,
            token: token);

        return client.JsonSerializer.DeserializeObject<CatalogResponse>(response.Body);
    
    }
}