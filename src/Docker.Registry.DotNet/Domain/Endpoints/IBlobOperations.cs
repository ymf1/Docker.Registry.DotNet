﻿//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
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

using Docker.Registry.DotNet.Domain.Blobs;

namespace Docker.Registry.DotNet.Domain.Endpoints;

[PublicAPI]
public interface IBlobOperations
{
    /// <summary>
    ///     Retrieve the blob from the registry identified by digest. Performs a monolithic download of the blob.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="digest"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [PublicAPI]
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
    [PublicAPI]
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