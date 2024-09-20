using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.DTOs.Chats;

public class ChatUpdateImageDto
{
    public IFormFile Image { get; set; } = null!;
}