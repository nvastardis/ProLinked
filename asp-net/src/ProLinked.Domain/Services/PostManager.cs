using ProLinked.Domain.Contracts.Posts;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Entities.Posts;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Posts;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Services;

public class PostManager: IPostManager
{
    private readonly IPostRepository _postRepository;
    private readonly IRepository<Reaction, Guid> _reactionRepository;
    private readonly IRepository<PostBlob> _postBlobRepository;
    private readonly IRepository<Comment, Guid> _commentRepository;


    public PostManager(
        IPostRepository postRepository,
        IRepository<Reaction, Guid> reactionRepository,
        IRepository<Comment, Guid> commentRepository,
        IRepository<PostBlob> postBlobRepository)
    {
        _postRepository = postRepository;
        _reactionRepository = reactionRepository;
        _commentRepository = commentRepository;
        _postBlobRepository = postBlobRepository;
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

    public async Task AddCommentAsync(
        Post post,
        Guid userId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null,
        CancellationToken cancellationToken = default)
    {
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
            post.Id,
            userId,
            parentId);
        newComment.SetContent(text, media);
        post.AddComment(newComment);
        await _commentRepository.InsertAsync(newComment, autoSave: true, cancellationToken);
    }

    public async Task UpdatePostAsync(
        Post post,
        string? text = null,
        List<Blob>? media = null,
        CancellationToken cancellationToken = default)
    {
        var blobList = new List<PostBlob>();
        if (media is not null && media.Count != 0)
        {
            blobList = media.ConvertAll(e => new PostBlob(post.Id, e.Id));
            await _postBlobRepository.DeleteManyAsync(e => e.PostId == post.Id, autoSave: true, cancellationToken);
            await _postBlobRepository.InsertManyAsync(blobList, autoSave: true, cancellationToken);
        }
        post.UpdatePost(text, blobList);
        await _postRepository.UpdateAsync(post, autoSave: true, cancellationToken);
    }

    public async Task UpdateCommentAsync(
        Comment comment,
        string? text = null,
        Blob? media = null,
        CancellationToken cancellationToken = default)
    {
        comment.UpdateComment(text, media);
        await _commentRepository.UpdateAsync(comment, autoSave: true, cancellationToken);
    }

    public async Task AddPostReactionAsync(
        Post post,
        Guid userId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        if (post.Reactions.Any(x => x.CreatorId == userId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionAlreadyExists)
                .WithData("UserId", userId);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newReaction = new Reaction(
            Guid.NewGuid(),
            userId,
            reactionType,
            postId:post.Id);

        post.AddReaction(newReaction);
        await _reactionRepository.InsertAsync(newReaction, autoSave: true, cancellationToken);
    }

    public async Task RemovePostReactionAsync(
        Post post,
        Reaction reaction,
        CancellationToken cancellationToken = default)
    {
        if (post.Reactions.All(e => e.Id != reaction.Id && e.CreatorId != reaction.CreatorId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionNotFound)
                .WithData("PostId", reaction.PostId!)
                .WithData("ReactionId", reaction.Id);
        }

        cancellationToken.ThrowIfCancellationRequested();
        post.RemoveReaction(reaction.Id);
        await _reactionRepository.DeleteAsync(reaction.Id, autoSave: true, cancellationToken);
    }


    public async Task AddCommentReactionAsync(
        Comment comment,
        Guid userId,
        ReactionTypeEnum reactionType,
        CancellationToken cancellationToken = default)
    {
        if (comment.Reactions.Any(e => e.CreatorId == userId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionAlreadyExists);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var newReaction = new Reaction(
            Guid.NewGuid(),
            userId,
            reactionType,
            commentId:comment.Id
        );

        comment.AddReaction(newReaction);
        await _reactionRepository.InsertAsync(newReaction, autoSave: true, cancellationToken);
    }

    public async Task RemoveCommentReactionAsync(
        Comment comment,
        Reaction reaction,
        CancellationToken cancellationToken = default)
    {
        if (comment.Reactions.All(x => x.Id != reaction.Id && x.CreatorId != reaction.CreatorId))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionNotFound)
                .WithData("ReactionId", reaction.Id)
                .WithData("CreatorId", reaction.CreatorId);
        }

        cancellationToken.ThrowIfCancellationRequested();
        comment.RemoveReaction(reaction.Id);
        await _reactionRepository.DeleteAsync(reaction, autoSave: true, cancellationToken);
    }


    public async Task SetVisibilityAsync(
        Post post,
        PostVisibilityEnum visibility,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        post.SetVisibility(visibility);
        await _postRepository.UpdateAsync(post, autoSave: true, cancellationToken);
    }

    public async Task<Post> GetPostAsCreatorAsync(
        Guid postId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var post =  await _postRepository.GetAsync(postId, includeDetails:true, cancellationToken);
        if (post.CreatorId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotPoster);
        }
        return post;
    }

    public async Task<Comment> GetCommentAsCreatorAsync(
        Guid commentId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var comment =  await _commentRepository.GetAsync(commentId, includeDetails:true, cancellationToken);
        if (comment.CreatorId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotPoster);
        }

        return comment;
    }

    public async Task<Reaction> GetPostReactionAsCreatorAsync(Guid reactionId, Guid userId, CancellationToken cancellationToken = default)
    {
        var reaction = await _reactionRepository.GetAsync(reactionId, includeDetails: false, cancellationToken);
        if (reaction.CreatorId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotPoster);
        }

        return reaction;
    }

    public async Task<Reaction> GetCommentReactionAsCreatorAsync(Guid reactionId, Guid userId, CancellationToken cancellationToken = default)
    {
        var reaction = await _reactionRepository.GetAsync(reactionId, includeDetails: false, cancellationToken);
        if (reaction.CreatorId != userId)
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.UserNotPoster);
        }

        return reaction;
    }
}