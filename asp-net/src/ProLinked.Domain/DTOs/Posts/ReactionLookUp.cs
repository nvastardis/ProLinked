using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Domain.DTOs.Posts;

public class ReactionLookUp
{
    public Guid CreatorId;
    public string CreatorFullName = null!;
    public Guid? CreatorProfilePhotoId;
    public DateTime CreationTime;
    public ReactionTypeEnum ReactionType;
}