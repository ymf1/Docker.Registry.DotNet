

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Manifests;

[DataContract]
public abstract class ImageManifest
{
    /// <summary>
    /// This field specifies the image manifest schema version as an integer.
    /// </summary>
    [DataMember(Name = "schemaVersion")]
    public int SchemaVersion { get; set; }
}