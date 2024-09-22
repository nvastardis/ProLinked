using ProLinked.Application.Contracts.Filtering;

namespace ProLinked.Application.Contracts.Posts.DTOs;

public record CommentListFilterDto: ListFilterDto
{
    public Guid? UserId;
    public Guid PostId;
}