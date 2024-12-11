

// 



// 

// 






namespace Docker.Registry.DotNet.Domain.Catalogs;


public class CatalogParameters
{
    /// <summary>
    ///     Limit the number of entries in each response. If it's not present, all entries will be returned.
    /// </summary>
    [QueryParameter("n")]
    public int? Number { get; set; }

    /// <summary>
    ///     Result set will include values lexically after last.
    /// </summary>
    [QueryParameter("last")]
    public int? Last { get; set; }

    public static CatalogParameters Empty { get; } = new();
}