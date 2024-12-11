

// 



// 

// 






using Docker.Registry.DotNet.Domain.Blobs;

namespace Docker.Registry.DotNet.Domain.Endpoints;


public interface IBlobOperations
{
    /// <summary>
    ///     Retrieve the blob from the registry identified by digest. Performs a monolithic download of the blob.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="digest"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task<GetBlobResponse> GetBlob(
        string name,
        string digest,
        CancellationToken token = default);

    /// <summary>
    ///     Delete the blob identified by name and digest.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="digest"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task DeleteBlob(
        string name,
        string digest,
        CancellationToken token = default);

    /// <summary>
    /// Existing Layers
    /// </summary>
    /// <param name="name"></param>
    /// <param name="digest"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> BlobExists(
        string name,
        string digest,
        CancellationToken token = default);
}