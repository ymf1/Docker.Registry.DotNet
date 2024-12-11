
namespace Docker.Registry.DotNet.Domain.Endpoints;

public interface ITagOperations
{
    Task<ListTagResponseModel> ListTags(
        string name,
        ListTagsParameters? parameters = null,
        CancellationToken token = default);

    Task<ListTagByDigestResponseModel> ListTagsByDigests(string name, CancellationToken token = default);
}