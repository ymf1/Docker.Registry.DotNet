

// 



// 

// 






using Docker.Registry.DotNet.Application.Endpoints;
using Docker.Registry.DotNet.Domain;
using Docker.Registry.DotNet.Domain.Configuration;
using Docker.Registry.DotNet.Domain.QueryStrings;

namespace Docker.Registry.DotNet.Application.Registry;

public class RegistryClient : IRegistryClient
{
    private static readonly TimeSpan _infiniteTimeout =
        TimeSpan.FromMilliseconds(Timeout.Infinite);

    private readonly HttpClient _client;

    private readonly IFrozenRegistryClientConfiguration _configuration;

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
        IFrozenRegistryClientConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (configuration.AuthenticationProvider == null)
            throw new ArgumentNullException(nameof(configuration.AuthenticationProvider));
        if (configuration.BaseAddress == null)
            throw new ArgumentNullException(nameof(configuration.BaseAddress));

        this._configuration = configuration;
        this._client = configuration.HttpMessageHandler is null
            ? new HttpClient()
            : new HttpClient(configuration.HttpMessageHandler);
        this.UriBuilder = new RegistryUriBuilder(configuration.BaseAddress);

        this.Manifest = new ManifestOperations(this);
        this.Catalog = new CatalogOperations(this);
        this.Blobs = new BlobOperations(this);
        this.BlobUploads = new BlobUploadOperations(this);
        this.System = new SystemOperations(this);
        this.Tags = new TagOperations(this);
        this.Repository = new RepositoryOperations(this);
    }

    private AuthenticationProvider AuthenticationProvider =>
        this._configuration.AuthenticationProvider;

    internal string RegistryVersion => DockerRegistryConstants.RegistryVersion;

    internal TimeSpan DefaultTimeout => this._configuration.DefaultTimeout;

    internal JsonSerializer JsonSerializer { get; } = new();

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

        using var activity = Assembly.Source.StartActivity("RegistryClient.InternalMakeRequestAsync()");

        var builtUri = this.UriBuilder.Build(path, queryString);

        var request = this.PrepareRequest(method, builtUri, headers, content);

        if (timeout != _infiniteTimeout)
        {
            var timeoutTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutTokenSource.CancelAfter(timeout);
            cancellationToken = timeoutTokenSource.Token;
        }

        await this.AuthenticationProvider.Authenticate(request, this.UriBuilder);

        activity?.AddEvent(new ActivityEvent($"Sending Request to: {request.RequestUri}"));

        var response = await this._client.SendAsync(
            request,
            completionOption,
            cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized) return response;

        activity?.AddEvent(new ActivityEvent("Authorization Challenged"));

        //Prepare another request (we can't reuse the same request)
        var request2 = this.PrepareRequest(method, builtUri, headers, content);

        //Authenticate given the challenge
        await this.AuthenticationProvider.Authenticate(request2, response, this.UriBuilder);

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

        request.Headers.Add("User-Agent", DockerRegistryConstants.Name);
        request.Headers.AddRange(headers);

        //Create the content
        request.Content = content?.Invoke();

        return request;
    }

    #region Operations

    public IRepositoryOperations Repository { get; set; }

    public IBlobUploadOperations BlobUploads { get; }

    public IManifestOperations Manifest { get; }

    public ICatalogOperations Catalog { get; }

    public IBlobOperations Blobs { get; }

    public ITagOperations Tags { get; }

    public ISystemOperations System { get; }

    #endregion
}