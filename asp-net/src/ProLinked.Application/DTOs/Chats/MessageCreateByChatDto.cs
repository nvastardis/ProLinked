using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.DTOs.Chats;

public class MessageCreateByChatDto
{
    public Guid ChatId { get; set; }
    public Guid? ParentId { get; set; }
    public string? Text { get; set; }
    public IFormFile? Media { get; set; }
}