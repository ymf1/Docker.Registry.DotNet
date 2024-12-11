// Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






global using System;
global using System.Diagnostics;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Runtime.Serialization;
global using System.Text;

global using Docker.Registry.DotNet.Application.Authentication;
global using Docker.Registry.DotNet.Application.Registry;
global using Docker.Registry.DotNet.Domain.Endpoints;
global using Docker.Registry.DotNet.Domain.Models;
global using Docker.Registry.DotNet.Domain.QueryParameters;
global using Docker.Registry.DotNet.Domain.Registry;
global using Docker.Registry.DotNet.Infrastructure.Helpers;

global using Newtonsoft.Json;

global using JsonSerializer = Docker.Registry.DotNet.Infrastructure.Json.JsonSerializer;