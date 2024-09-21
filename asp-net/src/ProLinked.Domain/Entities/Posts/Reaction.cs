using ProLinked.Domain.Shared;
using ProLinked.Domain.Shared.Exceptions;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Domain.Entities.Posts;

public class Reaction: Entity<Guid>
{
    public Guid? PostId { get; private set; }
    public Guid? CommentId { get; private set; }
    public Guid CreatorId { get; init; }
    public DateTime CreationTime { get; init; }
    public ReactionSourceEnum SourceType { get; private set; }
    public ReactionTypeEnum ReactionType { get; init; }

    private Reaction(Guid id): base(id){}

    public Reaction(
        Guid id,
        Guid creatorId,
        ReactionTypeEnum reactionType,
        Guid? postId = null,
        Guid? commentId = null,
        DateTime? creationTime = null)
        : base(id)
    {
        CreatorId = creatorId;
        ReactionType = reactionType;
        CreationTime = creationTime ?? DateTime.Now;
        SetSource(postId, commentId);
    }

    private void SetSource(Guid? postId, Guid? commentId)
    {
        var idArray = new [] {postId, commentId};

        if (
            idArray.All(e => !e.HasValue) ||
            idArray.All(e => e.HasValue && e.Value == Guid.Empty)
        )
        {
            throw new InvalidDataException();
        }

        if (postId.HasValue && postId.Value != Guid.Empty)
        {
            PostId = postId;
            SourceType = ReactionSourceEnum.POST;
        }
        else
        {
            CommentId = commentId;
            SourceType = ReactionSourceEnum.COMMENT;
        }
    }
}