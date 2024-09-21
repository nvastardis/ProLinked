using ProLinked.Application.DTOs.Filtering;

namespace ProLinked.Application.DTOs.Posts;

public record CommentListFilterDto: ListFilterDto
{
    public Guid? UserId;
    public Guid PostId;
}