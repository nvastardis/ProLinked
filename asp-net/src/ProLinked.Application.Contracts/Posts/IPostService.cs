using ProLinked.Application.Contracts.Filtering;
using ProLinked.Application.Contracts.Posts.DTOs;
using ProLinked.Application.DTOs;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.Contracts.Posts;

public interface IPostService
{
    /* Posts */
    Task<PagedAndSortedResultList<PostLookUpDto>> GetPostListAsync(
        PostListFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PagedAndSortedResultList<PostLookUpDto>> GetRecommendedPostListAsync(
        UserFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PostWithDetailsDto> GetWithDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task CreatePostAsync(
        PostCUDto input,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdatePostAsync(
        PostCUDto input,
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task SetVisibilityAsync(
        Guid postId,
        Guid userId,
        PostVisibilityEnum visibilityEnum,
        CancellationToken cancellationToken = default);

    Task DeletePostAsync(
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken);

    /* Post Reactions */
    Task<PagedAndSortedResultList<ReactionDto>> GetPostReactionListAsync(
        Guid postId,
        ListFilterDto filterDto,
        CancellationToken cancellationToken = default);

    Task CreatePostReactionAsync(
        Guid postId,
        Guid userId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default);

    Task DeletePostReactionAsync(
        Guid postId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default);
}