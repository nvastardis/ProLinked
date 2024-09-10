using ProLinked.Shared.Posts;

namespace ProLinked.Domain.Posts;

public class CommentReaction: Entity<Guid>
{
    public Guid CreatorId { get; init; }
    public Guid CommentId { get; init; }
    public ReactionTypeEnum ReactionType { get; init; }
    public DateTime CreationTime { get; init; }

    private CommentReaction(Guid id): base(id){}

    public CommentReaction(
        Guid id,
        Guid commentId,
        Guid creatorId,
        ReactionTypeEnum reactionType,
        DateTime? creationTime = null)
        : base(id)
    {
        CommentId = commentId;
        CreatorId = creatorId;
        ReactionType = reactionType;
        CreationTime = creationTime ?? DateTime.Now;
    }
}
