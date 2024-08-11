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