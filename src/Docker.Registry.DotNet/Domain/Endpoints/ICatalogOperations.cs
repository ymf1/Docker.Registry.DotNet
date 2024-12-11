
using Docker.Registry.DotNet.Domain.Catalogs;

namespace Docker.Registry.DotNet.Domain.Endpoints;

public interface ICatalogOperations
{
    Task<CatalogResponse> GetCatalog(
        CatalogParameters? parameters = null,
        CancellationToken token = default);
}