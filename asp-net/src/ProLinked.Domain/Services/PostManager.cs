using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Repositories.Posts;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Domain.Services;

public class PostManager: IPostManager
{
    private readonly IPostRepository _postRepository;
    private readonly IRepository<PostReaction, Guid> _postReactionRepository;
    private readonly IRepository<Comment, Guid> _commentRepository;
    private readonly IRepository<CommentReaction, Guid> _commentReactionRepository;


    public PostManager(
        IPostRepository postRepository,
        IRepository<PostReaction, Guid> postReactionRepository,
        IRepository<Comment, Guid> commentRepository,
        IRepository<CommentReaction, Guid> commentReactionRepository)
    {
        _postRepository = postRepository;
        _postReactionRepository = postReactionRepository;
        _commentRepository = commentRepository;
        _commentReactionRepository = commentReactionRepository;
    }

    public async Task<Post> CreatePostAsync(
        Guid userId,
        PostVisibilityEnum visibility,
        string? text = null,
        List<Blob>? media = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var post = new Post(
            Guid.NewGuid(),
            userId,
            visibility);

        if (text.IsNullOrWhiteSpace() && media?.Count == 0)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.PostCannotBeEmpty)
                .WithData("PostId", post.Id);
        }

        var blobList = new List<PostBlob>();
        if (media is not null && media.Any())
        {
            blobList = media.ConvertAll(e => new PostBlob(post.Id, e.Id));
        }
        post.SetContent(text, blobList);

        await _postRepository.InsertAsync(post, autoSave: true, cancellationToken);
        return post;
    }

    public async Task<Post> AddCommentAsync(
        Guid currentUserId,
        Guid postId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null,
        CancellationToken cancellationToken = default)
    {
        var post = await FindPostAsync(postId);
        var parent = post.Comments.FirstOrDefault(e => e.Id == (parentId ?? Guid.Empty));
        if (parentId.HasValue && parent is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ParentCommentNotFound)
                .WithData("PostId", post.Id)
                .WithData("ParentId", parentId);
        }

        if (text.IsNullOrWhiteSpace() && media is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.CommentCannotBeEmpty)
                .WithData("PostId", post.Id);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newComment = new Comment(
            Guid.NewGuid(),
            postId,
            currentUserId,
            parent);
        newComment.SetContent(text, media);
        post.AddComment(newComment);
        await _commentRepository.InsertAsync(newComment, autoSave: true, cancellationToken);
        return post;
    }

    public async Task<Post> AddPostReactionAsync(
        Guid currentUserId,
        Guid postId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        var post = await FindPostAsync(postId);

        if (post.Reactions.Any(x => x.CreatorId == currentUserId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionAlreadyExists)
                .WithData("UserId", currentUserId);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newReaction = new PostReaction(
            Guid.NewGuid(),
            postId,
            currentUserId,
            reactionType);
        post.AddReaction(newReaction);
        await _postReactionRepository.InsertAsync(newReaction, autoSave: true, cancellationToken);
        return post;
    }

    public async Task<Post> RemovePostReactionAsync(
        Guid postId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var post = await FindPostAsync(postId);
        if (post.Reactions.All(e => e.Id != reactionId && e.CreatorId != userId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionNotFound)
                .WithData("PostId", reactionId)
                .WithData("ReactionId", reactionId);
        }

        cancellationToken.ThrowIfCancellationRequested();
        post.RemoveReaction(reactionId);
        await _postReactionRepository.DeleteAsync(reactionId, autoSave: true, cancellationToken);

        return post;
    }


    public async Task<Post> AddCommentReactionAsync(
        Guid currentUserId,
        Guid postId,
        Guid commentId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        var post = await FindPostAsync(postId);
        var comment = FindComment(post, commentId);

        if (comment.Reactions.Any(e => e.CreatorId == currentUserId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionAlreadyExists);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newReaction = new CommentReaction(
            Guid.NewGuid(),
            commentId,
            currentUserId,
            reactionType
        );

        comment.AddReaction(newReaction);
        await _commentReactionRepository.InsertAsync(newReaction, autoSave: true, cancellationToken);

        return post;
    }

    public async Task<Post> RemoveCommentReactionAsync(
        Guid postId,
        Guid commentId,
        Guid reactionId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var post = await FindPostAsync(postId);
        var comment = FindComment(post, commentId);
        if (comment.Reactions.All(x => x.Id != reactionId && x.CreatorId != userId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionNotFound)
                .WithData("ReactionId", reactionId);
        }

        cancellationToken.ThrowIfCancellationRequested();
        comment.RemoveReaction(reactionId);
        await _commentReactionRepository.DeleteAsync(reactionId, autoSave: true, cancellationToken);

        return post;
    }


    public async Task<Post> SetVisibilityAsync(
        Guid postId,
        Guid userId,
        PostVisibilityEnum visibility,
        CancellationToken cancellationToken = default)
    {
        var post = await FindPostAsync(postId);
        if (post.CreatorId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.PostNotFound);
        }

        cancellationToken.ThrowIfCancellationRequested();
        post.SetVisibility(visibility);
        await _postRepository.UpdateAsync(post, autoSave: true, cancellationToken);
        return post;
    }

    private async Task<Post> FindPostAsync(Guid postId)
    {
        var post =  await _postRepository.FindAsync(x => x.Id == postId, includeDetails:true);
        if (post is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.PostNotFound)
                .WithData("PostId", postId);
        }

        return post;
    }

    private Comment FindComment(Post post, Guid commentId)
    {
        var commentsInPost = post.Comments;
        var comment = commentsInPost.FirstOrDefault(e => e.Id == commentId);
        if (comment is null)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.CommentNotFound)
                .WithData("PostId", post.Id)
                .WithData("CommentId", commentId);
        }

        return comment;
    }
}