

// 



// 

// 






using Docker.Registry.DotNet.Domain.Catalogs;

namespace Docker.Registry.DotNet.Domain.Endpoints;


public interface ICatalogOperations
{
    /// <summary>
    ///     Retrieve a sorted, json list of repositories available in the registry.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task<CatalogResponse> GetCatalog(
        CatalogParameters? parameters = null,
        CancellationToken token = default);
}