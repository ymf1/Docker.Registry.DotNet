
namespace Docker.Registry.DotNet.Application.Authentication;

internal class ParsedAuthentication(string? realm, string? service, string? scope)
{
    public string? Realm { get; } = realm;

    public string? Service { get; } = service;

    public string? Scope { get; } = scope;
}