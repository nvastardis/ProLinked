using System.Collections.ObjectModel;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Extensions;
using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Posts;
using ProLinked.Domain.Shared.Utils;

namespace ProLinked.Domain.Entities.Posts;

public class Comment: Entity<Guid>
{
    public Guid CreatorId { get; init; }
    public Guid PostId { get; init; }
    public ICollection<Reaction> Reactions { get; init; }
    public ICollection<Comment> Children { get; init; }
    public Guid? ParentId { get; init; }
    public string? Text { get; private set; }
    public Blob? Media { get; private set; }
    public DateTime CreationTime { get; init; }
    public DateTime? LastModificationTime { get; private set; }

    private Comment(Guid id): base(id){}

    public Comment(
        Guid id,
        Guid postId,
        Guid creatorId,
        Guid? parentId = null,
        string? text = null,
        Blob? media = null,
        DateTime? creationTime = null)
        : base(id)
    {
        PostId = postId;
        ParentId = parentId;
        CreatorId = creatorId;
        SetContent(text, media);
        Reactions = new Collection<Reaction>();
        Children = new Collection<Comment>();
        CreationTime = creationTime ?? DateTime.Now;
        LastModificationTime = null;
    }

    public Comment UpdateComment(string? text = null, Blob? media = null)
    {
        SetContent(text, media);
        LastModificationTime = DateTime.Now;
        return this;
    }

    public Comment SetContent(string? text = null, Blob? media = null)
    {
        if (!text.IsNullOrWhiteSpace())
        {
            SetText(text!);
        }

        if (media is not null)
        {
            SetMedia(media);
        }
        return this;
    }

    public Comment AddReaction(Reaction reactionToAdd)
    {
        if (Reactions.Any(x => x.CreatorId == reactionToAdd.CreatorId || x.Id == reactionToAdd.Id))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionAlreadyExists)
                .WithData(nameof(Reaction.CreatorId), reactionToAdd.CreatorId)
                .WithData(nameof(Reaction.Id), reactionToAdd.Id);
        }

        Reactions.Add(reactionToAdd);

        return this;
    }

    public Comment RemoveReaction(Guid? reactionId)
    {
        var reactionToDelete = Reactions.FirstOrDefault(e => e.Id == reactionId);
        Reactions.Remove(reactionToDelete!);
        return this;
    }

    public Comment AddChild(Comment newComment)
    {
        Children.Add(newComment);
        return this;
    }

    public Comment RemoveChild(Comment comment)
    {
        Children.Remove(comment);
        return this;
    }

    private void SetText(string text)
    {
        Text = Check.NotNullOrWhiteSpace(
            text,
            nameof(text),
            CommentConsts.MaxContentLength,
            CommentConsts.MinContentLength);
    }
    private void SetMedia(Blob media)
    {
        Media = media;
    }
}