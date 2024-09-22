using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.Contracts.Chats.DTOs;

public class MessageCreateDto
{
    public Guid? ParentId { get; set; }
    public string? Text { get; set; }
    public IFormFile? Media { get; set; }
}