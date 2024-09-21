using ProLinked.Application.DTOs.Filtering;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.DTOs.Posts;

public record PostListFilterDto: UserFilterDto
{
    public PostVisibilityEnum VisibilityEnum;
}