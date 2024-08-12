//  Copyright 2017-2022 Rich Quackenbush, Jaben Cargman
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

namespace Docker.Registry.DotNet.Application.Authentication;

/// <summary>
///     Authentication provider.
/// </summary>
public abstract class AuthenticationProvider
{
    /// <summary>
    ///     Called on initial connection
    /// </summary>
    /// <param name="request"></param>
    /// <param name="uriBuilder"></param>
    /// <returns></returns>
    public abstract Task Authenticate(HttpRequestMessage request, IRegistryUriBuilder uriBuilder);

    /// <summary>
    ///     Called when connection is challenged.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public abstract Task Authenticate(
        HttpRequestMessage request,
        HttpResponseMessage response,
        IRegistryUriBuilder builder);

    /// <summary>
    ///     Gets the schema header from the http response.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    protected AuthenticationHeaderValue TryGetSchemaHeader(
        HttpResponseMessage response,
        string schema)
    {
        var header = response.GetHeaderBySchema(schema);

        if (header == null)
            throw new InvalidOperationException(
                $"No WWW-Authenticate challenge was found for schema {schema}");

        return header;
    }
}