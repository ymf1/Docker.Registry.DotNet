

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Manifests;

[DataContract]
public class ManifestSignatureHeader
{
    [DataMember(Name = "alg")]
    public string? Alg { get; set; }
}