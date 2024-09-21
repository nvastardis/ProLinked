using ProLinked.Application.DTOs;
using ProLinked.Application.DTOs.Filtering;
using ProLinked.Application.DTOs.Posts;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.Contracts.Posts;

public interface IPostService
{
    Task<PagedAndSortedResultList<PostLookUpDto>> GetPostListAsync(
        PostListFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<CommentDto>> GetCommentListAsync(
        Guid postId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<CommentReactionDto>> GetCommentReactionListAsync(
        Guid commentId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<PostReactionDto>> GetPostReactionListAsync(
        Guid postId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default);

    Task<PostWithDetailsDto> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task CreatePostAsync(
        PostCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    // TODO ?
    // Task UpdatePostAsync(
    //     Guid postId,
    //     PostCUDto input,
    //     Guid userId,
    //     CancellationToken cancellationToken = default);

    Task SetVisibilityAsync(
        Guid postId,
        PostVisibilityEnum visibilityEnum,
        Guid userId,
        CancellationToken cancellationToken = default);


    Task AddPostReactionAsync(
        Guid postId,
        ReactionTypeEnum reactionType,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RemovePostReactionAsync(
        Guid postId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddCommentAsync(
        Guid postId,
        CommentCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task AddCommentReactionAsync(
        Guid postId,
        Guid commentId,
        ReactionTypeEnum reactionType,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task RemoveCommentReactionAsync(
        Guid postId,
        Guid commentId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default);
}