// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 







// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






using Docker.Registry.DotNet.Domain.ImageReferences;
using Docker.Registry.DotNet.Domain.Manifests;

namespace Docker.Registry.DotNet.Domain.Endpoints;

/// <summary>
///     Manifest operations.
/// </summary>

public interface IManifestOperations
{
    /// <summary>
    ///     Delete the manifest.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task DeleteManifest(string name, ImageReference reference, CancellationToken token = default);

    /// <summary>
    ///     Fetch the manifest identified by name and reference raw.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<string> GetManifestRaw(
        string name,
        ImageReference reference,
        CancellationToken token = default);

    Task<ImageDigest?> GetDigest(string name, ImageTag tag, CancellationToken token = default);

    /// <summary>
    ///     Fetch the manifest identified by name and reference where reference can be a tag or digest. A HEAD request can also
    ///     be issued to this endpoint to obtain resource information without receiving all data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    
    Task<GetImageManifestResult> GetManifest(string name, ImageReference reference, CancellationToken token = default);

    /// <summary>
    ///     Put the manifest identified by name and reference where reference can be a tag or digest.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="manifest"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<PushManifestResponse> PutManifest(
        string name,
        ImageReference reference,
        ImageManifest manifest,
        CancellationToken token = default);
}