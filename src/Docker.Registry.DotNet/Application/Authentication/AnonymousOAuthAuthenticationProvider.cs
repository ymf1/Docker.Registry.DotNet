

// 



// 

// 






using Docker.Registry.DotNet.Application.OAuth;

namespace Docker.Registry.DotNet.Application.Authentication;


public class AnonymousOAuthAuthenticationProvider : AuthenticationProvider
{
    private readonly OAuthClient _client = new();

    private static string Schema { get; } = "Bearer";

    public override Task Authenticate(HttpRequestMessage request, IRegistryUriBuilder uriBuilder)
    {
        using var activity = Assembly.Source.StartActivity("AnonymousOAuthAuthenticationProvider.Authenticate(request)");

        return Task.CompletedTask;
    }

    public override async Task Authenticate(
        HttpRequestMessage request,
        HttpResponseMessage response,
        IRegistryUriBuilder uriBuilder)
    {
        using var activity = Assembly.Source.StartActivity("AnonymousOAuthAuthenticationProvider.Authenticate(request, response)");

        var header = this.TryGetSchemaHeader(response, Schema);

        //Get the bearer bits
        var bearerBits = AuthenticateParser.ParseTyped(header.Parameter);

        //Get the token
        var token = await this._client.GetToken(
            bearerBits.Realm,
            bearerBits.Service,
            bearerBits.Scope);

        if (token?.Token == null)
        {
            throw new ArgumentNullException(nameof(token.Token), "Authorization token cannot be null");
        }

        //Set the header
        request.Headers.Authorization = new AuthenticationHeaderValue(Schema, token.Token);
    }
}