using ProLinked.Domain;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Shared;

namespace ProLinked.Application.Services.Posts;

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
