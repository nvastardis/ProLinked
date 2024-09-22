using ProLinked.Application.Contracts.Filtering;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.Contracts.Posts.DTOs;

public record PostListFilterDto: ListFilterDto
{
    public Guid? UserId = null;
    public PostVisibilityEnum VisibilityEnum;
}