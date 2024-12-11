

// 



// 

// 






using System.Runtime.CompilerServices;

using Docker.Registry.DotNet.Domain;

[assembly: InternalsVisibleTo("Docker.Registry.DotNet.Tests")]

namespace Docker.Registry.DotNet;

internal sealed class Assembly
{
    internal static ActivitySource Source = new(
        DockerRegistryConstants.Name,
        DockerRegistryConstants.Version);
}