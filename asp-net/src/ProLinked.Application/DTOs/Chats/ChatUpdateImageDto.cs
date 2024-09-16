using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.DTOs.Chats;

public class ChatUpdateImageDto: EntityDto<Guid>
{
    public IFormFile Image { get; set; } = null!;
}