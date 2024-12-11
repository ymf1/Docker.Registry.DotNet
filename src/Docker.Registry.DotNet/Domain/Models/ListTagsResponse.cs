// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






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