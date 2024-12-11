

// 



// 

// 






using Docker.Registry.DotNet.Domain.Blobs;
using Docker.Registry.DotNet.Domain.ImageReferences;

namespace Docker.Registry.DotNet.Domain.Endpoints;


public interface IBlobUploadOperations
{
    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="contentLength"></param>
    /// <param name="stream"></param>
    /// <param name="digest"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task UploadBlob(
        string name,
        int contentLength,
        Stream stream,
        ImageDigest digest,
        CancellationToken token = default);

    /// <summary>
    ///     Mount a blob identified by the mount parameter from another repository.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parameters"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task<MountResponse> MountBlob(
        string name,
        MountParameters parameters,
        CancellationToken token = default);

    /// <summary>
    ///     Retrieve status of upload identified by uuid. The primary purpose of this endpoint is to resolve the current status
    ///     of a resumable upload.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="uuid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    
    Task<BlobUploadStatus> GetBlobUploadStatus(
        string name,
        string uuid,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Upload a chunk of data for the specified upload.
    /// </summary>
    /// <param name="resumable"></param>
    /// <param name="chunk"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task<ResumableUpload> UploadBlobChunk(
        ResumableUpload resumable,
        Stream chunk,
        long? from = null,
        long? to = null,
        CancellationToken token = default);

    /// <summary>
    ///     Complete the upload specified by ResumableUploadResponse, optionally appending the body as the final chunk.
    /// </summary>
    /// <param name="resumable"></param>
    /// <param name="digest"></param>
    /// <param name="chunk"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task<CompletedUploadResponse> CompleteBlobUpload(
        ResumableUpload resumable,
        ImageDigest digest,
        Stream? chunk = null,
        long? from = null,
        long? to = null,
        CancellationToken token = default);

    /// <summary>
    ///     Cancel outstanding upload processes, releasing associated resources. If this is not called, the unfinished uploads
    ///     will eventually timeout.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="uuid"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task CancelBlobUpload(
        string name,
        string uuid,
        CancellationToken token = default);

    /// <summary>
    /// Starting An Upload
    /// </summary>
    /// <param name="name"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<ResumableUpload> StartUploadBlob(
        string name,
        CancellationToken token = default);

    /// <summary>
    /// A monolithic upload is simply a chunked upload with a single chunk and may be favored by clients that would like to avoided the complexity of chunking
    /// </summary>
    /// <param name="resumable"></param>
    /// <param name="digest"></param>
    /// <param name="stream"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<CompletedUploadResponse> MonolithicUploadBlob(
        ResumableUpload resumable,
        ImageDigest digest,
        Stream stream,
        CancellationToken token = default);
}