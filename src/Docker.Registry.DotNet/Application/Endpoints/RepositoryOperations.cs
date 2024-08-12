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

using Docker.Registry.DotNet.Application.QueryStrings;
using Docker.Registry.DotNet.Domain.Repository;

namespace Docker.Registry.DotNet.Application.Endpoints;

internal class RepositoryOperations(RegistryClient client) : IRepositoryOperations
{
    public async Task<ListRepositoryTagsResponse> ListRepositoryTags(
        string @namespace,
        string repository,
        RepositoryTagsParameters? parameters = null,
        CancellationToken token = default)
    {
        var queryString = new QueryString();
        queryString.AddFromObject(parameters ?? new RepositoryTagsParameters());

        var response = await client.MakeRequest(
            HttpMethod.Get,
            $"{client.RegistryVersion}/namespaces/{@namespace}/repositories/{repository}/tags",
            queryString,
            token: token);

        return client.JsonSerializer.DeserializeObject<ListRepositoryTagsResponse>(response.Body);
    }
}