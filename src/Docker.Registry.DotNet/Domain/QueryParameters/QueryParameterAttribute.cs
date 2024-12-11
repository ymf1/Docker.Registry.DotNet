
namespace Docker.Registry.DotNet.Domain.QueryParameters;

[AttributeUsage(AttributeTargets.Property)]
internal class QueryParameterAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}