// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






using Docker.Registry.DotNet.Application.Endpoints;
using Docker.Registry.DotNet.Domain.Repository;

namespace Docker.Registry.DotNet.Domain.Endpoints;

/// <summary>
/// Operations on the Docker Repository.
/// </summary>
public interface IRepositoryOperations
{
    Task<ListRepositoryTagsResponse> ListRepositoryTags(
        string @namespace,
        string repository,
        RepositoryTagsParameters? parameters = null,
        CancellationToken token = default);
}