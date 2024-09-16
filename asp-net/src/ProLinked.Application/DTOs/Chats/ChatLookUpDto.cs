using Microsoft.AspNetCore.Http;

namespace ProLinked.Application.DTOs.Chats;

public class ChatLookUpDto: EntityDto<Guid>
{
    public string Title { get; set; } = null!;
    public Guid? ImageId { get; set; }
    public string? LastMessageContent { get; set; }
    public string? LastMessageSenderName { get; set; }
    public DateTime LastMessageDate { get; set; }
}