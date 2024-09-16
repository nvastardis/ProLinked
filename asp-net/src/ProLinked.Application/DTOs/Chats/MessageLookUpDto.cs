using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.DTOs.Chats;

public class MessageLookUpDto: EntityDto<Guid>
{
    public Guid SenderId { get; set; }
    public string SenderFullName { get; set; } = null!;
    public string? Text { get; set; }
    public Guid? MediaId { get; set; }
}