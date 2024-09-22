namespace ProLinked.Application.Contracts.Posts.DTOs;

public class PostWithDetailsDto
{
    public Guid Id;
    public Guid CreatorId;
    public string CreatorFullName = null!;
    public Guid? CreatorProfilePhotoId;
    public DateTime CreationTime;
    public int CommentCount = 0;
    public List<CommentDto>? Comments;
    public int ReactionCount = 0;
    public List<ReactionDto>? Reactions;
    public string? Text = null;
    public List<Guid>? MediaIds = null;
    public DateTime? LastModificationTime = null;
}