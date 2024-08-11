//  Copyright 2017-2024 Rich Quackenbush, Jaben Cargman
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

[PublicAPI]
public class BasicAuthenticationProvider(string username, string password) : AuthenticationProvider
{
    private static string Schema { get; } = "Basic";

    public override Task Authenticate(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    public override Task Authenticate(
        HttpRequestMessage request,
        HttpResponseMessage response,
        IRegistryUriBuilder uriBuilder)
    {
        this.TryGetSchemaHeader(response, Schema);

        var passBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
        var base64Pass = Convert.ToBase64String(passBytes);

        //Set the header
        request.Headers.Authorization =
            new AuthenticationHeaderValue(Schema, base64Pass);

        return Task.CompletedTask;
    }
}