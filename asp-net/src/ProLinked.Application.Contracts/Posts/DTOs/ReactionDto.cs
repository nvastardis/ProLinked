using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.Contracts.Posts.DTOs;

public class ReactionDto
{
    public Guid CreatorId;
    public string CreatorFullName = null!;
    public Guid? CreatorProfilePhotoId;
    public DateTime CreationTime;
    public ReactionTypeEnum ReactionType;
}