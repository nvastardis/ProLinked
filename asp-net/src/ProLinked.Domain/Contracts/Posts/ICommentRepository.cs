using ProLinked.Domain.DTOs.Posts;
using ProLinked.Domain.Entities.Posts;

namespace ProLinked.Domain.Contracts.Posts;

public interface ICommentRepository: IRepository<Comment,Guid>
{
    Task<List<Comment>> GetListAsync(
        Guid? postId = null,
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool includeDetails = false,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

    Task<List<CommentLookUp>> GetLookUpListAsync(
        Guid? postId = null,
        Guid? userId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? sorting = null,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);

    Task<List<ReactionLookUp>> GetReactionsAsync(
        Guid commentId,
        int skipCount = ProLinkedConsts.SkipCountDefaultValue,
        int maxResultCount = ProLinkedConsts.MaxResultCountDefaultValue,
        CancellationToken cancellationToken = default);
}