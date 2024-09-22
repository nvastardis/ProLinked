using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.Contracts.Posts.DTOs;

public class CommentCUDto
{
    public Guid PostId { get; set; }
    public Guid? ParentId;
    public string? Text;
    public IFormFile? Media { get; set; }
}