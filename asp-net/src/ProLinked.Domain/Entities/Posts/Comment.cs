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
    public ICollection<CommentReaction> Reactions { get; init; }
    public Comment? Parent { get; init; }
    public string? Text { get; private set; }
    public Blob? Media { get; private set; }
    public DateTime CreationTime { get; init; }

    private Comment(Guid id): base(id){}

    public Comment(
        Guid id,
        Guid postId,
        Guid creatorId,
        Comment? parent = null,
        string? text = null,
        Blob? media = null,
        DateTime? creationTime = null)
        : base(id)
    {
        PostId = postId;
        Parent = parent;
        CreatorId = creatorId;
        SetContent(text, media);
        Reactions = new Collection<CommentReaction>();
        CreationTime = creationTime ?? DateTime.Now;
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

    public Comment AddReaction(CommentReaction reactionToAdd)
    {
        if (Reactions.Any(x => x.CreatorId == reactionToAdd.CreatorId || x.Id == reactionToAdd.Id))
        {
            throw new BusinessException(ProLinkedDomainErrorCodes.ReactionAlreadyExists)
                .WithData(nameof(CommentReaction.CreatorId), reactionToAdd.CreatorId)
                .WithData(nameof(CommentReaction.Id), reactionToAdd.Id);
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
