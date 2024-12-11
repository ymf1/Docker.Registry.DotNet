

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Blobs;


public class BlobHeader
{
    internal BlobHeader(string dockerContentDigest)
    {
        this.DockerContentDigest = dockerContentDigest;
    }

    public string DockerContentDigest { get; }
}