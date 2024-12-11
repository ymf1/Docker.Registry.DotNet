

// 



// 

// 






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