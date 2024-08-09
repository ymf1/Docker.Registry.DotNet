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

using System.Diagnostics;

using JsonSerializer = Docker.Registry.DotNet.Helpers.JsonSerializer;

namespace Docker.Registry.DotNet.Registry;

internal class NetworkClient : IDisposable
{
    private const string UserAgent = "Docker.Registry.DotNet";

    private static readonly TimeSpan InfiniteTimeout =
        TimeSpan.FromMilliseconds(Timeout.Infinite);

    private readonly AuthenticationProvider _authenticationProvider;

    private readonly HttpClient _client;

    private readonly RegistryClientConfiguration _configuration;

    private readonly IEnumerable<Action<RegistryApiResponse>> _errorHandlers =
        new Action<RegistryApiResponse>[]
        {
            r =>
            {
                if (r.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedApiException(r);
            }
        };

    internal IRegistryUriBuilder? UriBuilder;

    public NetworkClient(
        RegistryClientConfiguration configuration,
        AuthenticationProvider authenticationProvider)
    {
        this._configuration =
            configuration ?? throw new ArgumentNullException(nameof(configuration));

        this._authenticationProvider = authenticationProvider
                                       ?? throw new ArgumentNullException(
                                           nameof(authenticationProvider));

        this._client = configuration.HttpMessageHandler is null
            ? new HttpClient()
            : new HttpClient(configuration.HttpMessageHandler);

        this.DefaultTimeout = configuration.DefaultTimeout;

        this.JsonSerializer = new JsonSerializer();
    }

    public string RegistryVersion { get; } = "v2";

    public TimeSpan DefaultTimeout { get; set; }

    public JsonSerializer JsonSerializer { get; }

    public void Dispose()
    {
        this._client.Dispose();
    }

    /// <summary>
    ///     Ensures that we have configured (and potentially probed) the end point.
    /// </summary>
    /// <returns></returns>
    private async Task EnsureConnection()
    {
        if (this.UriBuilder != null) return;

        var tryUrls = new List<string>();

        // clean up the host
        var host = this._configuration.Host.ToLower().Trim();

        if (host.StartsWith("http"))
        {
            // includes schema -- don't add
            tryUrls.Add(host);
        }
        else
        {
            tryUrls.Add($"https://{host}");
            tryUrls.Add($"http://{host}");
        }

        var exceptions = new List<Exception>();

        foreach (var url in tryUrls)
            try
            {
                var registryUriBuilder = new RegistryUriBuilder(url);
                await this.ProbeSingleEndpoint(registryUriBuilder);
                this.UriBuilder = registryUriBuilder;
                return;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

        throw new RegistryConnectionException(
            $"Unable to connect to any: {tryUrls.Select(s => $"'{s}/{this.RegistryVersion}/'").ToDelimitedString(", ")}'",
            new AggregateException(exceptions));
    }

    private async Task ProbeSingleEndpoint(IRegistryUriBuilder uriBuilder)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Build());
        using (await this._client.SendAsync(request))
        {
        }
    }

    internal async Task<RegistryApiResponse<string>> MakeRequest(
        HttpMethod method,
        string path,
        IReadOnlyQueryString? queryString = null,
        IDictionary<string, string>? headers = null,
        Func<HttpContent>? content = null,
        CancellationToken token = default)
    {
        using var response = await this.InternalMakeRequestAsync(
            this.DefaultTimeout,
            HttpCompletionOption.ResponseContentRead,
            method,
            path,
            queryString,
            headers,
            content,
            token);

        var responseBody = await response.Content.ReadAsStringAsyncWithCancellation(token);

        var apiResponse = new RegistryApiResponse<string>(
            response.StatusCode,
            responseBody,
            response.Headers);

        this.HandleIfErrorResponse(apiResponse);

        return apiResponse;
    }

    internal async Task<RegistryApiResponse<string>> MakeRequestNotErrorAsync(
        HttpMethod method,
        string path,
        IReadOnlyQueryString? queryString = null,
        IDictionary<string, string>? headers = null,
        Func<HttpContent>? content = null,
        CancellationToken token = default)
    {
        using var response = await this.InternalMakeRequestAsync(
            this.DefaultTimeout,
            HttpCompletionOption.ResponseContentRead,
            method,
            path,
            queryString,
            headers,
            content,
            token);

        var responseBody = await response.Content.ReadAsStringAsyncWithCancellation(token)
            .ConfigureAwait(false);

        var apiResponse = new RegistryApiResponse<string>(
            response.StatusCode,
            responseBody,
            response.Headers);

        return apiResponse;
    }

    internal async Task<RegistryApiResponse<Stream>> MakeRequestForStreamedResponseAsync(
        HttpMethod method,
        string path,
        IReadOnlyQueryString? queryString = null,
        CancellationToken token = default)
    {
        var response = await this.InternalMakeRequestAsync(
            InfiniteTimeout,
            HttpCompletionOption.ResponseHeadersRead,
            method,
            path,
            queryString,
            null,
            null,
            token);

        var body = await response.Content.ReadAsStreamAsyncWithCancellation(token);

        var apiResponse = new RegistryApiResponse<Stream>(
            response.StatusCode,
            body,
            response.Headers);

        this.HandleIfErrorResponse(apiResponse);

        return apiResponse;
    }

    private async Task<HttpResponseMessage> InternalMakeRequestAsync(
        TimeSpan timeout,
        HttpCompletionOption completionOption,
        HttpMethod method,
        string path,
        IReadOnlyQueryString? queryString,
        IDictionary<string, string>? headers,
        Func<HttpContent>? content,
        CancellationToken cancellationToken)
    {
        await this.EnsureConnection();

        if (this.UriBuilder == null)
            throw new ArgumentNullException(nameof(this.UriBuilder), "Could not find URI builder");

        var builtUri = this.UriBuilder.Build(path, queryString);

        Debug.WriteLine("Built URI: " + builtUri);

        var request = this.PrepareRequest(method, builtUri, headers, content);

        if (timeout != InfiniteTimeout)
        {
            var timeoutTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutTokenSource.CancelAfter(timeout);
            cancellationToken = timeoutTokenSource.Token;
        }

        await this._authenticationProvider.Authenticate(request);

        var response = await this._client.SendAsync(
            request,
            completionOption,
            cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized) return response;

        //Prepare another request (we can't reuse the same request)
        var request2 = this.PrepareRequest(method, builtUri, headers, content);

        //Authenticate given the challenge
        await this._authenticationProvider.Authenticate(request2, response, this.UriBuilder);

        //Send it again
        response = await this._client.SendAsync(
            request2,
            completionOption,
            cancellationToken);

        return response;
    }

    internal void HandleIfErrorResponse(RegistryApiResponse<string> response)
    {
        // If no customer handlers just default the response.
        foreach (var handler in this._errorHandlers) handler(response);

        // No custom handler was fired. Default the response for generic success/failures.
        if (response.StatusCode is < HttpStatusCode.OK or >= HttpStatusCode.BadRequest)
            throw new RegistryApiException<string>(response);
    }

    internal void HandleIfErrorResponse(RegistryApiResponse response)
    {
        // If no customer handlers just default the response.
        foreach (var handler in this._errorHandlers) handler(response);

        // No custom handler was fired. Default the response for generic success/failures.
        if (response.StatusCode is < HttpStatusCode.OK or >= HttpStatusCode.BadRequest)
            throw new RegistryApiException(response);
    }

    internal HttpRequestMessage PrepareRequest(
        HttpMethod method,
        Uri uri,
        IDictionary<string, string>? headers,
        Func<HttpContent>? content)
    {
        var request = new HttpRequestMessage(
            method,
            uri);

        request.Headers.Add("User-Agent", UserAgent);
        request.Headers.AddRange(headers);

        //Create the content
        request.Content = content?.Invoke();

        return request;
    }
}