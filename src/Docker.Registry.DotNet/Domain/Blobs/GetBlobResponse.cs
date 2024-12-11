

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Blobs;

public class GetBlobResponse : BlobHeader, IDisposable
{
    internal GetBlobResponse(string dockerContentDigest, Stream stream)
        : base(dockerContentDigest)
    {
            this.Stream = stream;
        }

    public Stream Stream { get; }

    public void Dispose()
    {
            this.Stream?.Dispose();
        }
}