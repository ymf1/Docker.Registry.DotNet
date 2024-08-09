// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
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

using Docker.Registry.DotNet.Domain.Tags;

namespace Docker.Registry.DotNet.Endpoints;

/// <summary>
///     Manifest operations.
/// </summary>
[PublicAPI]
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
    Task<string?> GetManifestRaw(string name, ImageReference reference, CancellationToken token = default);

    Task<ImageDigest?> GetDigest(string name, ImageTag tag, CancellationToken token = default);

    /// <summary>
    ///     Fetch the manifest identified by name and reference where reference can be a tag or digest. A HEAD request can also
    ///     be issued to this endpoint to obtain resource information without receiving all data.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="reference"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [PublicAPI]
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