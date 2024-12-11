// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Endpoints;


public interface ITagOperations
{
    Task<ListTagResponseModel> ListTags(
        string name,
        ListTagsParameters? parameters = null,
        CancellationToken token = default);

    Task<ListTagByDigestResponseModel> ListTagsByDigests(string name, CancellationToken token = default);
}