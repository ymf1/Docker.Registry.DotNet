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

namespace Docker.Registry.DotNet.Endpoints;

[PublicAPI]
public interface IBlobUploadOperations
{
    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="contentLength"></param>
    /// <param name="stream"></param>
    /// <param name="digest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [PublicAPI]
    Task UploadBlobAsync(
        string name,
        int contentLength,
        Stream stream,
        string digest,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Initiate a resumable blob upload. If successful, an upload location will be provided to complete the upload.
    ///     Optionally, if the digest parameter is present, the request body will be used to complete the upload in a single
    ///     request.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [PublicAPI]
    Task<ResumableUpload> InitiateBlobUploadAsync(
        string name,
        Stream? stream = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Mount a blob identified by the mount parameter from another repository.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [PublicAPI]
    Task<MountResponse> MountBlobAsync(
        string name,
        MountParameters parameters,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieve status of upload identified by uuid. The primary purpose of this endpoint is to resolve the current status
    ///     of a resumable upload.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="uuid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [PublicAPI]
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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [PublicAPI]
    Task<ResumableUpload> UploadBlobChunkAsync(
        ResumableUpload resumable,
        Stream chunk,
        long? from = null,
        long? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Complete the upload specified by ResumableUploadResponse, optionally appending the body as the final chunk.
    /// </summary>
    /// <param name="resumable"></param>
    /// <param name="digest"></param>
    /// <param name="chunk"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [PublicAPI]
    Task<CompletedUploadResponse> CompleteBlobUploadAsync(
        ResumableUpload resumable,
        string digest,
        Stream? chunk = null,
        long? from = null,
        long? to = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Cancel outstanding upload processes, releasing associated resources. If this is not called, the unfinished uploads
    ///     will eventually timeout.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="uuid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [PublicAPI]
    Task CancelBlobUploadAsync(
        string name,
        string uuid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Starting An Upload
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ResumableUpload> StartUploadBlobAsync(
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// A monolithic upload is simply a chunked upload with a single chunk and may be favored by clients that would like to avoided the complexity of chunking
    /// </summary>
    /// <param name="resumable"></param>
    /// <param name="digest"></param>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CompletedUploadResponse> MonolithicUploadBlobAsync(
        ResumableUpload resumable,
        string digest,
        Stream stream,
        CancellationToken cancellationToken = default);
}