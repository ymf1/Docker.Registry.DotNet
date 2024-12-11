﻿

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Models;

[DataContract]
public class MountParameters
{
    /// <summary>
    /// Digest of blob to mount from the source repository.
    /// </summary>
    public string? Digest { get; set; }

    /// <summary>
    /// Name of the source repository.
    /// </summary>
    public string? From { get; set; }
}