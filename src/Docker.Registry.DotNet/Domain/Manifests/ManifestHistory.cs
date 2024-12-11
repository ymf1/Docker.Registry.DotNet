

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Manifests;

[DataContract]
public class ManifestHistory
{
    [DataMember(Name = "v1Compatibility")]
    public string? V1Compatibility { get; set; }
}