// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






using Docker.Registry.DotNet.Application.QueryStrings;
using Docker.Registry.DotNet.Domain.ImageReferences;

namespace Docker.Registry.DotNet.Application.Endpoints;

internal class TagOperations(RegistryClient client) : ITagOperations
{
    public async Task<ListTagResponseModel> ListTags(
        string name,
        ListTagsParameters? parameters = null,
        CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));

        parameters ??= new ListTagsParameters();

        var queryString = new QueryString();

        queryString.AddFromObject(parameters);

        var response = await client.MakeRequest(
            HttpMethod.Get,
            $"{client.RegistryVersion}/{name}/tags/list",
            queryString,
            token: token);

        var listTags = client.JsonSerializer.DeserializeObject<ListTagsResponseDto?>(response.Body);

        return listTags == null
            ? ListTagResponseModel.Empty
            : new ListTagResponseModel(
                listTags.Name ?? string.Empty,
                listTags.Tags.Select(ImageTag.Create).ToHashSet());
    }

    public async Task<ListTagByDigestResponseModel> ListTagsByDigests(string name, CancellationToken token = default)
    {
        var tags = await ListTags(name, token: token);

        var manifestOperations = new ManifestOperations(client);

        var digestLookup = new Dictionary<ImageDigest, HashSet<ImageTag>>();

        var tasks = tags.Tags.Select(async t => (Tag: t, Digest: await manifestOperations.GetDigest(name, t, token)))
            .ToList();

        var tagDigestList = (await Task.WhenAll(tasks)).Where(s => s.Digest != null).ToList();

        foreach (var item in tagDigestList)
        {
            if (item.Digest != null)
            {
                if (digestLookup.TryGetValue(item.Digest, out var list))
                {
                    list.Add(item.Tag);
                }
                else
                {
                    digestLookup.Add(item.Digest, [item.Tag]);
                }
            }
        }

        return new ListTagByDigestResponseModel(
            name,
            digestLookup.Select(s => new DigestTagModel(s.Key, digestLookup[s.Key].ToHashSet())).ToList());
    }
}