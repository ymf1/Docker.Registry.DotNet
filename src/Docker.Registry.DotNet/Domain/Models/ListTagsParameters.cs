

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Models;

public class ListTagsParameters
{
    /// <summary>
    ///     Limit the number of entries in each response. Default is all entries.
    /// </summary>
    [QueryParameter("n")]
    public int? Number { get; set; }
}