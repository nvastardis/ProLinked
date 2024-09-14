using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Domain.Contracts.Posts;

public interface IPostManager
{
    Task<Post> CreatePostAsync(
        Guid userId,
        PostVisibilityEnum visibility,
        string? text = null,
        List<Blob>? media = null,
        CancellationToken cancellationToken = default);

    Task<Post> AddCommentAsync(
        Guid currentUserId,
        Guid postId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null,
        CancellationToken cancellationToken = default);

    Task<Post> AddPostReactionAsync(
        Guid currentUserId,
        Guid postId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default);

    Task<Post> RemovePostReactionAsync(
        Guid postId,
        Guid reactionId,
        CancellationToken cancellationToken = default);

    Task<Post> AddCommentReactionAsync(
        Guid currentUserId,
        Guid postId,
        Guid commentId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default);

    Task<Post> RemoveCommentReactionAsync(
        Guid postId,
        Guid commentId,
        Guid reactionId,
        CancellationToken cancellationToken = default);

    Task<Post> SetVisibilityAsync(
        Guid postId,
        PostVisibilityEnum visibility,
        CancellationToken cancellationToken = default);


}