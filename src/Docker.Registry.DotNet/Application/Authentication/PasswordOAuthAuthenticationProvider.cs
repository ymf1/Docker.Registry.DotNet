

// 



// 

// 






using Docker.Registry.DotNet.Application.OAuth;

namespace Docker.Registry.DotNet.Application.Authentication;


public class PasswordOAuthAuthenticationProvider(string username, string password)
    : AuthenticationProvider
{
    private readonly OAuthClient _client = new();

    private static string Schema { get; } = "Bearer";

    public override Task Authenticate(HttpRequestMessage request, IRegistryUriBuilder uriBuilder)
    {
        using var activity = Assembly.Source.StartActivity("PasswordOAuthAuthenticationProvider.Authenticate(request)");

        return Task.CompletedTask;
    }

    public override async Task Authenticate(
        HttpRequestMessage request,
        HttpResponseMessage response,
        IRegistryUriBuilder uriBuilder)
    {
        using var activity = Assembly.Source.StartActivity("PasswordOAuthAuthenticationProvider.Authenticate(request, response)");

        var header = this.TryGetSchemaHeader(response, Schema);

        //Get the bearer bits
        var bearerBits = AuthenticateParser.ParseTyped(header.Parameter);

        string? scope = null;

        if (!string.IsNullOrWhiteSpace(bearerBits.Scope))
            //Also include the repository(plugin) resource type to be able to access plugin repositories.
            //See https://docs.docker.com/registry/spec/auth/scope/
            scope =
                $"{bearerBits.Scope} {bearerBits.Scope?.Replace("repository:", "repository(plugin):")}";

        //Get the token
        var token = await this._client.GetToken(
            bearerBits.Realm,
            bearerBits.Service,
            scope,
            username,
            password);
        
        if (token?.AccessToken == null)
        {
            throw new ArgumentNullException(nameof(token.AccessToken), "Authorization token cannot be null");
        }

        //Set the header
        request.Headers.Authorization =
            new AuthenticationHeaderValue(Schema, token.AccessToken);
    }
}