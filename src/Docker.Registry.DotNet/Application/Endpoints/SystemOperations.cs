
namespace Docker.Registry.DotNet.Application.Endpoints;

internal class SystemOperations(RegistryClient client) : ISystemOperations
{
    public virtual Task Ping(CancellationToken token = default)
    {
        return client.MakeRequest(HttpMethod.Get, "", token: token);
    }
}