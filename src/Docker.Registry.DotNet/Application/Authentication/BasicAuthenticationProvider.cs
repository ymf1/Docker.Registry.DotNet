//  Copyright 2017-2024 Rich Quackenbush, Jaben Cargman

// 



// 

// 






namespace Docker.Registry.DotNet.Application.Authentication;


public class BasicAuthenticationProvider(string username, string password) : AuthenticationProvider
{
    private static string Schema { get; } = "Basic";

    public override Task Authenticate(HttpRequestMessage request, IRegistryUriBuilder uriBuilder)
    {
        using var activity = Assembly.Source.StartActivity("BasicAuthenticationProvider.Authenticate(request)");

        return Task.CompletedTask;
    }

    public override Task Authenticate(
        HttpRequestMessage request,
        HttpResponseMessage response,
        IRegistryUriBuilder uriBuilder)
    {
        using var activity = Assembly.Source.StartActivity("BasicAuthenticationProvider.Authenticate(request, response)");

        this.TryGetSchemaHeader(response, Schema);

        var passBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
        var base64Pass = Convert.ToBase64String(passBytes);

        //Set the header
        request.Headers.Authorization =
            new AuthenticationHeaderValue(Schema, base64Pass);

        return Task.CompletedTask;
    }
}