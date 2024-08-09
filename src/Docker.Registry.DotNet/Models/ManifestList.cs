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

namespace Docker.Registry.DotNet.Models;

/// <summary>
///     The manifest list is the “fat manifest” which points to specific image manifests for one or more platforms. Its use
///     is optional, and relatively few images will use one of these manifests. A client will distinguish a manifest list
///     from an image manifest based on the Content-Type returned in the HTTP response.
/// </summary>
public class ManifestList : ImageManifest
{
    /// <summary>
    ///     The MIME type of the manifest list. This should be set to
    ///     application/vnd.docker.distribution.manifest.list.v2+json.
    /// </summary>
    [DataMember(Name = "mediaType")]
    public string? MediaType { get; set; }

    /// <summary>
    ///     The manifests field contains a list of manifests for specific platforms.
    /// </summary>
    [DataMember(Name = "manifests")]
    public Manifest[]? Manifests { get; set; }
}