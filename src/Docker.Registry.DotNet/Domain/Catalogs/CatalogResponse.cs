
namespace Docker.Registry.DotNet.Domain.Catalogs;

[DataContract]
public class CatalogResponse
{
    [DataMember(Name = "repositories", EmitDefaultValue = false)]
    public IReadOnlyCollection<string> Repositories { get; set; } = [];
}