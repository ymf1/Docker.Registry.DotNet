// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 







/* Unmerged change from project 'Docker.Registry.DotNet (netstandard2.0)'
Before:
namespace Docker.Registry.DotNet.Registry;
After:
using Docker;
using Docker.Registry;
using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Domain.Registry;
using Docker.Registry.DotNet.Registry;

namespace Docker.Registry.DotNet.Registry;
*/

// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






using Docker.Registry.DotNet.Domain.QueryStrings;

namespace Docker.Registry.DotNet.Domain.Registry;

public interface IRegistryUriBuilder
{
    Uri Build(string? path = null, string? queryString = null);
}

internal static class RegistryUriBuilderExtensions
{
    public static Uri Build(
        this IRegistryUriBuilder uriBuilder,
        string? path = null,
        IReadOnlyQueryString? queryString = null)
    {
        return uriBuilder.Build(path, queryString?.GetQueryString());
    }
}