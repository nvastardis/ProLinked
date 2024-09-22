using Microsoft.AspNetCore.Http;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.Contracts.Posts.DTOs;

public class PostCUDto
{
    public PostVisibilityEnum Visibility;
    public string? Text;
    public FormFileCollection? Media;
}