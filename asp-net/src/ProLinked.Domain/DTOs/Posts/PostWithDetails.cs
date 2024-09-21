namespace ProLinked.Domain.DTOs.Posts;

public class PostWithDetails
{
    public Guid Id;
    public Guid CreatorId;
    public string CreatorFullName = null!;
    public Guid? CreatorProfilePhotoId;
    public DateTime CreationTime;
    public int CommentCount = 0;
    public List<CommentLookUp>? Comments;
    public int ReactionCount = 0;
    public List<ReactionLookUp>? Reactions;
    public string? Text = null;
    public List<Guid>? MediaIds = null;
    public DateTime? LastModificationTime = null;
}