namespace Docker.Registry.DotNet.Domain.Repository;

[PublicAPI]
public class RepositoryTagsParameters
{
    /// <summary>
    ///     Current page.
    /// </summary>
    [QueryParameter("page")]
    public int Page { get; set; } = 1;

    /// <summary>
    ///     Page Size -- max is 100
    /// </summary>
    [QueryParameter("page_size")]
    public int PageSize { get; set; } = 10;
}