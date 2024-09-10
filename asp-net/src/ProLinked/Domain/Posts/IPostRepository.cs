using ProLinked.Shared;

namespace ProLinked.Domain.Posts;

public interface IPostRepository: IRepository<Post, Guid>
{
    Task<List<Post>> GetListByUserAsync(
        Guid userId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

}
