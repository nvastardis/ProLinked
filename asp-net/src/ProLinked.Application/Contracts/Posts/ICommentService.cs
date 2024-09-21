using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Application.DTOs.Posts;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.Contracts.Posts;

public interface ICommentService
{
    /* Comments */
    Task<PagedAndSortedResultList<CommentDto>> GetCommentListForPostAsync(
        CommentListFilterDto filterDto,
        CancellationToken cancellationToken = default);

    Task CreateCommentAsync(
        CommentCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateCommentAsync(
        CommentCUDto input,
        Guid commentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task DeleteCommentAsync(
        Guid commentId,
        Guid userId,
        CancellationToken cancellationToken);

    /* Comment Reactions */
    Task<PagedAndSortedResultList<ReactionDto>> GetCommentReactionListAsync(
        Guid commentId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default);

    Task CreateCommentReactionAsync(
        Guid commentId,
        Guid userId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default);

    Task DeleteCommentReactionAsync(
        Guid commentId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default);
}