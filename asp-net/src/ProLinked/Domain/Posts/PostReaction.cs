using ProLinked.Shared.Posts;

namespace ProLinked.Domain.Posts;

public class PostReaction: Entity<Guid>
{
    public Guid PostId { get; init; }
    public Guid? CreatorId { get; init; }
    public DateTime CreationTime { get; init; }
    public ReactionTypeEnum ReactionType { get; init; }

    private PostReaction(Guid id): base(id){}

    public PostReaction(
        Guid id,
        Guid postId,
        Guid creatorId,
        ReactionTypeEnum reactionType,
        DateTime? creationTime = null)
        : base(id)
    {
        PostId = postId;
        CreatorId = creatorId;
        ReactionType = reactionType;
        CreationTime = creationTime ?? DateTime.Now;
    }

}
