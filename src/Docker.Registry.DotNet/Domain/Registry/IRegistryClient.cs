

// 



// 

// 









// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Registry;

/// <summary>
///     The registry client.
/// </summary>
public interface IRegistryClient : IDisposable
{
    /// <summary>
    ///     Manifest operations
    /// </summary>
    
    IManifestOperations Manifest { get; }

    /// <summary>
    ///     Catalog operations
    /// </summary>
    
    ICatalogOperations Catalog { get; }

    /// <summary>
    ///     Blog operations
    /// </summary>
    
    IBlobOperations Blobs { get; }

    /// <summary>
    /// Blob Upload operations
    /// </summary>
    
    IBlobUploadOperations BlobUploads { get; }

    /// <summary>
    ///     Tag operations
    /// </summary>
    
    ITagOperations Tags { get; }

    /// <summary>
    ///     System operations
    /// </summary>
    
    ISystemOperations System { get; }

    IRepositoryOperations Repository { get; }
}