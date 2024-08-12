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

using Docker.Registry.DotNet.Domain.Blobs;
using Docker.Registry.DotNet.Domain.Catalogs;
using Docker.Registry.DotNet.Domain.ImageReferences;
using Docker.Registry.DotNet.Domain.Manifests;

namespace Docker.Registry.DotNet;

public static class RegistryClientLegacyHelpers
{
    [Obsolete("Use Ping() instead")]
    public static Task PingAsync(
        this ISystemOperations operations,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.Ping(token);
    }

    [Obsolete("Use BlobExists() instead")]
    public static Task IsExistBlobAsync(
        this IBlobOperations operations,
        string name,
        string digest,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.BlobExists(name, digest, token);
    }

    [Obsolete("Use DeleteBlob() instead")]
    public static Task DeleteBlobAsync(
        this IBlobOperations operations,
        string name,
        string digest,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.DeleteBlob(name, digest, token);
    }

    [Obsolete("Use GetBlob() instead")]
    public static Task<GetBlobResponse> GetBlobAsync(
        this IBlobOperations operations,
        string name,
        string digest,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.GetBlob(name, digest, token);
    }

    [Obsolete("Use GetCatalog() instead")]
    public static Task<CatalogResponse> GetCatalogAsync(
        this ICatalogOperations operations,
        CatalogParameters? parameters = null,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.GetCatalog(parameters ?? CatalogParameters.Empty, token);
    }

    [Obsolete("Use GetManifest() instead")]
    public static Task<GetImageManifestResult> GetManifestAsync(
        this IManifestOperations operations,
        string name,
        string reference,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.GetManifest(name, ImageReference.Create(reference), token);
    }

    [Obsolete("Use DeleteManifest() instead")]
    public static Task DeleteManifestAsync(
        this IManifestOperations operations,
        string name,
        string reference,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.DeleteManifest(name, ImageReference.Create(reference), token);
    }

    [Obsolete("Use PutManifest() instead")]
    public static Task PutManifestAsync(
        this IManifestOperations operations,
        string name,
        string reference,
        ImageManifest manifest,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.PutManifest(name, ImageReference.Create(reference), manifest, token);
    }

    [Obsolete("Use GetManifestRaw() instead")]
    public static Task<string> GetManifestRawAsync(
        this IManifestOperations operations,
        string name,
        string reference,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.GetManifestRaw(name, ImageReference.Create(reference), token);
    }

    [Obsolete("Use ListTags() instead")]
    public static Task<ListTagResponseModel> ListImageTagsAsync(
        this ITagOperations operations,
        string name,
        ListTagsParameters? parameters = null,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.ListTags(name, parameters, token);
    }

    [Obsolete("Use MonolithicUploadBlob() instead")]
    public static Task<CompletedUploadResponse> MonolithicUploadBlobAsync(
        this IBlobUploadOperations operations,
        ResumableUpload resumable,
        string digest,
        Stream stream,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.MonolithicUploadBlob(
            resumable,
            ImageDigest.Create(digest),
            stream,
            token);
    }

    [Obsolete("Use UploadBlob() instead")]
    public static Task UploadBlobAsync(
        this IBlobUploadOperations operations,
        string name,
        int contentLength,
        Stream stream,
        string digest,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.UploadBlob(
            name,
            contentLength,
            stream,
            ImageDigest.Create(digest),
            token);
    }

    [Obsolete("Use MountBlob() instead")]
    public static Task MountBlobAsync(
        this IBlobUploadOperations operations,
        string name,
        MountParameters parameters,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.MountBlob(name, parameters, token);
    }

    [Obsolete("Use UploadBlobChunk() instead")]
    public static Task UploadBlobChunkAsync(
        this IBlobUploadOperations operations,
        ResumableUpload resumable,
        Stream chunk,
        long? from = null,
        long? to = null,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.UploadBlobChunk(resumable, chunk, from, to, token);
    }

    [Obsolete("Use CompleteBlobUpload() instead")]
    public static Task CompleteBlobUploadAsync(
        this IBlobUploadOperations operations,
        ResumableUpload resumable,
        string digest,
        Stream chunk,
        long? from = null,
        long? to = null,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.CompleteBlobUpload(
            resumable,
            ImageDigest.Create(digest),
            chunk,
            from,
            to,
            token);
    }

    [Obsolete("Use CancelBlobUpload() instead")]
    public static Task CancelBlobUploadAsync(
        this IBlobUploadOperations operations,
        string name,
        string uuid,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.CancelBlobUpload(name, uuid, token);
    }

    [Obsolete("Use StartUploadBlob() instead")]
    public static Task StartUploadBlobAsync(
        this IBlobUploadOperations operations,
        string name,
        CancellationToken token = default)
    {
        if (operations == null) throw new ArgumentNullException(nameof(operations));

        return operations.StartUploadBlob(name, token);
    }
}