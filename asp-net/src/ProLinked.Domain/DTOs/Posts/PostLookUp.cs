namespace ProLinked.Domain.DTOs.Posts;

public class PostLookUp
{
    public Guid Id;
    public Guid CreatorId;
    public string CreatorFullName = null!;
    public Guid? CreatorProfilePhotoId;
    public DateTime CreationTime;
    public int ReactionCount = 0;
    public int CommentCount = 0;
    public string? Text = null;
    public List<Guid>? MediaIds = null;
    public DateTime? LastModificationTime = null;
}