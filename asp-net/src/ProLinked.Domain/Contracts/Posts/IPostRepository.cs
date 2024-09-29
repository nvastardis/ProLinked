using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Entities.Recommendations;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Domain.Contracts.Posts;

public interface IPostRepository: IRepository<Post, Guid>
{
    Task<PostWithDetails> GetWithDetailsAsync(
        Guid postId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

    Task<List<PostLookUp>> GetRecommendedAsync(
        Guid userId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

    Task<List<PostLookUp>> GetLookUpListAsync(
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        PostVisibilityEnum visibilityEnum = PostVisibilityEnum.UNDEFINED,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

    Task<List<ReactionLookUp>> GetReactionsAsync(
        Guid postId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

}