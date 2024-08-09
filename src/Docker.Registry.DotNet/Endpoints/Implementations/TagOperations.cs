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

using Docker.Registry.DotNet.Domain.Tags;

namespace Docker.Registry.DotNet.Endpoints.Implementations;

internal class TagOperations(NetworkClient client) : ITagOperations
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
                listTags.Tags.Select(ImageReference.Create).ToHashSet());
    }
}