using Microsoft.AspNetCore.Http;
using ProLinked.Domain.Entities.Blobs;
using ProLinked.Domain.Shared.Posts;

namespace ProLinked.Application.DTOs.Posts;

public class PostCUDto
{
    public PostVisibilityEnum Visibility;
    public string? Text;
    public FormFileCollection? Media;
}