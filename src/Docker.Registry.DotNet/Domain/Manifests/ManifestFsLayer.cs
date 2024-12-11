

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Manifests;

[DataContract]
public class ManifestFsLayer
{
    [DataMember(Name = "blobSum")]
    public string? BlobSum { get; set; }
}