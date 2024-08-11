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

using Docker.Registry.DotNet.Domain.ImageReferences;

namespace Docker.Registry.DotNet.Domain.Models;

[DataContract]
internal class ListTagsResponseDto
{
    [DataMember(Name = "name")]
    public string? Name { get; set; }

    [DataMember(Name = "tags")]
    public IReadOnlyCollection<string> Tags { get; set; } = [];
}

public record ListTagResponseModel(string Name, IReadOnlyCollection<ImageTag> Tags)
{
    public static ListTagResponseModel Empty { get; } = new("", []);
}

public record DigestTagModel(ImageDigest Digest, IReadOnlyCollection<ImageTag> Tags);

public record ListTagByDigestResponseModel(string Name, IReadOnlyCollection<DigestTagModel> Tags)
{
    public static ListTagByDigestResponseModel Empty { get; } = new("", new List<DigestTagModel>());
}