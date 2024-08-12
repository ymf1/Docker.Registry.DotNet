# ![docker icon](https://raw.githubusercontent.com/ChangemakerStudios/Docker.Registry.DotNet/develop/assets/docker_registry_dotnet-24p.png) Docker.Registry.DotNet

[![NuGet version](https://badge.fury.io/nu/Docker.Registry.DotNet.svg)](https://badge.fury.io/nu/Docker.Registry.DotNet)
[![Build and Push to Nuget](https://github.com/ChangemakerStudios/Docker.Registry.DotNet/actions/workflows/publish.yml/badge.svg)](https://github.com/ChangemakerStudios/Docker.Registry.DotNet/actions/workflows/publish.yml)
[![Downloads](https://img.shields.io/nuget/dt/Docker.Registry.DotNet.svg?logo=nuget&color=purple)](https://www.nuget.org/packages/Docker.Registry.DotNet)

.NET (C#) Client Library for interacting with a Docker Registry API (v2 only).

# Setup
*Install nuget package into your project via powershell:*
```powershell
PM> Install-Package Docker.Registry.DotNet
```

*Add nuget package via dotnet CLI:*
```
dotnet add package Docker.Registry.DotNet
```

# Usage

### Local Hub

```csharp
var configuration = new RegistryClientConfiguration("http://localhost:5000");

//configuration.UsePasswordOAuthAuthentication("username", "password")

using (var client = configuration.CreateClient())
{
    // get catalog
    var catalog = await client.Catalog.GetCatalog();

    // list tags for the first catalog
    var tags = await client.Tags.ListTags(catalog?.Repositories.FirstOrDefault());
}
```

### Remote Hub with Authentication

```csharp
var configuration = new RegistryClientConfiguration("https://proget.mycompany.com");

configuration.UsePasswordOAuthAuthentication("username", "password")

using (var client = configuration.CreateClient())
{
    // get catalog
    var catalog = await client.Catalog.GetCatalog();

    // list tags for the first catalog
    var tags = await client.Tags.ListTags(catalog?.Repositories.FirstOrDefault());
}
```

### Docker Hub

```csharp
var configuration = new RegistryClientConfiguration("https://hub.docker.com");

using (var client = configuration.CreateClient())
{
    // load respository
    var tags = await client.Repository.ListRepositoryTags("grafana", "loki-docker-driver");
}
```