namespace ProLinked.Application.Contracts.Posts.DTOs;

public class CommentDto
{
    public Guid Id;
    public Guid PostId;
    public Guid CreatorId;
    public string CreatorFullName = null!;
    public Guid? CreatorProfilePhotoId;
    public DateTime CreationTime;
    public int ReactionCount = 0;
    public int RepliesCount = 0;
    public string? Text = null;
    public Guid? MediaId = null;
    public Guid? ParentId = null;
    public DateTime? LastModificationTime = null;
}