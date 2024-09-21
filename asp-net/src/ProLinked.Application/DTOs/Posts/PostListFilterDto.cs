using ProLinked.Application.DTOs.Filtering;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.DTOs.Posts;

public record PostListFilterDto: ListFilterDto
{
    public Guid? UserId = null;
    public PostVisibilityEnum VisibilityEnum;
}