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

    Task<Comment> AddCommentAsync(
        Post post,
        Guid userId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null,
        CancellationToken cancellationToken = default);

    Task UpdatePostAsync(
        Post post,
        string? text = null,
        List<Blob>? media = null,
        CancellationToken cancellationToken = default);

    Task UpdateCommentAsync(
        Comment comment,
        string? text = null,
        Blob? media = null,
        CancellationToken cancellationToken = default);

    Task<Reaction> AddPostReactionAsync(
        Post post,
        Guid userId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default);

    Task RemovePostReactionAsync(
        Post post,
        Reaction reaction,
        CancellationToken cancellationToken = default);

    Task<Reaction> AddCommentReactionAsync(
        Comment comment,
        Guid userId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default);

    Task RemoveCommentReactionAsync(
        Comment comment,
        Reaction reaction,
        CancellationToken cancellationToken = default);

    Task SetVisibilityAsync(
        Post post,
        PostVisibilityEnum visibility,
        CancellationToken cancellationToken = default);

    Task<Post> GetPostAsCreatorAsync(
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Comment> GetCommentAsCreatorAsync(
        Guid commentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Reaction> GetPostReactionAsCreatorAsync(
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Reaction> GetCommentReactionAsCreatorAsync(
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default);
}