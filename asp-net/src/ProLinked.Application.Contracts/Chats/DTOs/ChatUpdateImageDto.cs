using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.Contracts.Chats.DTOs;

public class ChatUpdateImageDto
{
    public IFormFile Image { get; set; } = null!;
}