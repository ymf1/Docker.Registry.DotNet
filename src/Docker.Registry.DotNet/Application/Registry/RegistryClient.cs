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

using Docker.Registry.DotNet.Application.Endpoints;
using Docker.Registry.DotNet.Domain;
using Docker.Registry.DotNet.Domain.QueryStrings;

namespace Docker.Registry.DotNet.Application.Registry;

public class RegistryClient : IRegistryClient
{
    private static readonly TimeSpan _infiniteTimeout =
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

    public RegistryClient(
        RegistryClientConfiguration configuration,
        AuthenticationProvider authenticationProvider)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (authenticationProvider == null) throw new ArgumentNullException(nameof(authenticationProvider));
        if (configuration.BaseAddress == null) throw new ArgumentNullException(nameof(configuration.BaseAddress));

        this._authenticationProvider = authenticationProvider;
        this._client = configuration.HttpMessageHandler is null
            ? new HttpClient()
            : new HttpClient(configuration.HttpMessageHandler);
        this._configuration = configuration;
        this.UriBuilder = new RegistryUriBuilder(configuration.BaseAddress);
        
        this.Manifest = new ManifestOperations(this);
        this.Catalog = new CatalogOperations(this);
        this.Blobs = new BlobOperations(this);
        this.BlobUploads = new BlobUploadOperations(this);
        this.System = new SystemOperations(this);
        this.Tags = new TagOperations(this);
        this.Repository = new RepositoryOperations(this);
    }

    internal string RegistryVersion => DockerRegistryConstants.RegistryVersion;

    internal TimeSpan DefaultTimeout => this._configuration.DefaultTimeout;

    internal JsonSerializer JsonSerializer { get; } = new();

    #region Operations

    public IRepositoryOperations Repository { get; set; }

    public IBlobUploadOperations BlobUploads { get; }

    public IManifestOperations Manifest { get; }

    public ICatalogOperations Catalog { get; }

    public IBlobOperations Blobs { get; }

    public ITagOperations Tags { get; }

    public ISystemOperations System { get; } 

    #endregion

    public void Dispose()
    {
        this._client.Dispose();
    }

    internal async Task<RegistryApiResponse<string>> MakeRequest(
        HttpMethod method,
        string path,
        IReadOnlyQueryString? queryString = null,
        IReadOnlyDictionary<string, string>? headers = null,
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
        IReadOnlyDictionary<string, string>? headers = null,
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
            _infiniteTimeout,
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
        IReadOnlyDictionary<string, string>? headers,
        Func<HttpContent>? content,
        CancellationToken cancellationToken)
    {
        if (this.UriBuilder == null)
            throw new ArgumentNullException(nameof(this.UriBuilder), "Could not find URI builder");

        var builtUri = this.UriBuilder.Build(path, queryString);

        Debug.WriteLine($"Built URI: {builtUri}");

        var request = this.PrepareRequest(method, builtUri, headers, content);

        if (timeout != _infiniteTimeout)
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
        IReadOnlyDictionary<string, string>? headers,
        Func<HttpContent>? content)
    {
        var request = new HttpRequestMessage(method, uri);

        request.Headers.Add("User-Agent", DockerRegistryConstants.UserAgent);
        request.Headers.AddRange(headers);

        //Create the content
        request.Content = content?.Invoke();

        return request;
    }
}