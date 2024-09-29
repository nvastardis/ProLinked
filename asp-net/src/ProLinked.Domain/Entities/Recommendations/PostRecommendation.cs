using ProLinked.Domain.Entities.Posts;

namespace ProLinked.Domain.Entities.Recommendations;

public class PostRecommendation: Entity
{
    public Guid UserId { get; init; }
    public Guid PostId { get; init; }
    public virtual Post? Post { get; init; }

    private PostRecommendation() { }

    public PostRecommendation(
        Guid userId,
        Guid postId)
    {
        UserId = userId;
        PostId = postId;
    }


    public override object?[] GetKeys()
    {
        return [UserId, PostId];
    }
}