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

namespace Docker.Registry.DotNet.Authentication;

public class DockerHubJwtAuthenticationProvider(string username, string password)
    : AuthenticationProvider
{
    private static readonly HttpClient _client = new();

    private static string Schema { get; } = "Bearer";

    public override Task Authenticate(HttpRequestMessage request)
    {
        return Task.CompletedTask;
    }

    private async Task<OAuthToken?> PostAuth(
        IRegistryUriBuilder uriBuilder,
        CancellationToken token = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Build("/v2/users/login"))
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(
                    new Dictionary<string, string>
                    {
                        { "username", username },
                        { "password", password }
                    })
            )
        };

        using var response = await _client.SendAsync(request, token);

        if (!response.IsSuccessStatusCode)
            throw new UnauthorizedAccessException(
                $"Unable to authenticate: {await response.Content.ReadAsStringAsyncWithCancellation(token)}");

        var body = await response.Content.ReadAsStringAsyncWithCancellation(token);

        var authToken = JsonConvert.DeserializeObject<OAuthToken>(body);

        return authToken;
    }

    public override async Task Authenticate(
        HttpRequestMessage request,
        HttpResponseMessage response,
        IRegistryUriBuilder uriBuilder)
    {
        var tokenResponse = await this.PostAuth(uriBuilder);

        if (tokenResponse?.Token == null)
            throw new UnauthorizedAccessException("Failed to authenticate. Token was empty.");

        //Set the header
        request.Headers.Authorization =
            new AuthenticationHeaderValue(Schema, tokenResponse.Token);
    }
}