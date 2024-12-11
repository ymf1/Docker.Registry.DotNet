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






namespace Docker.Registry.DotNet.Domain.Registry;

internal abstract class RegistryApiResponse(HttpStatusCode statusCode, HttpResponseHeaders headers)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public HttpResponseHeaders Headers { get; } = headers;
}

internal class RegistryApiResponse<TBody> : RegistryApiResponse
{
    internal RegistryApiResponse(HttpStatusCode statusCode, TBody? body, HttpResponseHeaders headers)
        : base(statusCode, headers)
    {
        Body = body;
    }

    public TBody? Body { get; }
}