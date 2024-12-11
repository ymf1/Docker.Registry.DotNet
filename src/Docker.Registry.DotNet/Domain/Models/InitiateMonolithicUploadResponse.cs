

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Models;

public class InitiateMonolithicUploadResponse
{
    public string? Location { get; set; }

    public int ContentLength { get; set; }

    public string? DockerUploadUuid { get; set; }
}