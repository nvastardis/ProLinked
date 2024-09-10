using ProLinked.Domain.AzureStorage.Blobs;
using ProLinked.Shared;
using ProLinked.Shared.Exceptions;
using ProLinked.Shared.Extensions;
using ProLinked.Shared.Posts;
using ProLinked.Shared.Utils;

namespace ProLinked.Domain.Posts;

public class PostManager: IDomainService
{
    private readonly IPostRepository _postRepository;

    public PostManager(
        IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Post> CreatePostAsync(
        Guid userId,
        PostVisibilityEnum visibility,
        string? text = null,
        List<Blob>? media = null)
    {
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
        return await Task.FromResult(post);
    }

    public async Task<Post> AddCommentAsync(
        Guid currentUserId,
        Guid postId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null)
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

        var newComment = new Comment(
            Guid.NewGuid(),
            postId,
            currentUserId,
            parent);
        newComment.SetContent(text, media);
        post.AddComment(newComment);
        return post;
    }

    public async Task<Post> AddPostReactionAsync(
        Guid currentUserId,
        Guid postId,
        ReactionTypeEnum reactionType)
    {
        var post = await FindPostAsync(postId);

        if (post.Reactions.Any(x => x.CreatorId == currentUserId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionAlreadyExists)
                .WithData("UserId", currentUserId);
        }

        var newReaction = new PostReaction(
            Guid.NewGuid(),
            postId,
            currentUserId,
            reactionType);
        post.AddReaction(newReaction);

        return post;
    }

    public async Task<Post> RemovePostReactionAsync(Guid postId, Guid reactionId)
    {
        var post = await FindPostAsync(postId);
        if (post.Reactions.All(e => e.Id != reactionId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionNotFound)
                .WithData("PostId", reactionId)
                .WithData("ReactionId", reactionId);
        }
        post.RemoveReaction(reactionId);

        return post;
    }


    public async Task<Post> AddCommentReactionAsync(
        Guid currentUserId,
        Guid postId,
        Guid commentId,
        ReactionTypeEnum reactionType)
    {
        var post = await FindPostAsync(postId);
        var comment = FindComment(post, commentId);

        if (comment.Reactions.Any(e => e.CreatorId == currentUserId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionAlreadyExists);
        }

        var newReaction = new CommentReaction(
            Guid.NewGuid(),
            commentId,
            currentUserId,
            reactionType
        );

        comment.AddReaction(newReaction);

        return post;
    }

    public async Task<Post> RemoveCommentReactionAsync(
        Guid postId,
        Guid commentId,
        Guid reactionId)
    {
        var post = await FindPostAsync(postId);
        var comment = FindComment(post, commentId);
        if (comment.Reactions.All(x => x.Id != reactionId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionNotFound)
                .WithData("ReactionId", reactionId);
        }
        comment.RemoveReaction(reactionId);

        return post;
    }


    public async Task<Post> SetVisibilityAsync(
        Guid postId,
        PostVisibilityEnum visibility)
    {
        var post = await FindPostAsync(postId);
        post.SetVisibility(visibility);
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
