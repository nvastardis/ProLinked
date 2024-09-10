using System.Collections.ObjectModel;
using ProLinked.Domain.AzureStorage.Blobs;
using ProLinked.Shared;
using ProLinked.Shared.Exceptions;
using ProLinked.Shared.Extensions;
using ProLinked.Shared.Posts;
using ProLinked.Shared.Utils;

namespace ProLinked.Domain.Posts;

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

    internal Comment SetContent(string? text = null, Blob? media = null)
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

    internal Comment AddReaction(CommentReaction reactionToAdd)
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

    internal Comment RemoveReaction(Guid? reactionId)
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
